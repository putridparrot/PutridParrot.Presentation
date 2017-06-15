I've implemented view models (for the MVVM pattern) in so many different ways over the years 
and discovered each has its own merits. Basically, there isn't a great "one size fits all" approach,
in my opinion. Hence this solution contains various base classes which you can derive your 
view model from, each building upon the functionality of the other. 

If speed is your main concern, you might simply roll your own INotifyPropertyChanged implementation,
in which cases these classes only offer some shared functionality which could be of use. If you want to 
be close to the metal you can use the NotifyPropertyChanged class as your base, 
along with extension methods (I think they were originally inspired by Reactive UI, I cannot recall now). 

*One of the main aims of these classes is to remove "plumbing" code from your view model, hopefully leaving 
your code cleaner and more maintainable.*

So with just the NotifyPropertyChanged implementations our view model might look like this

```
public class PersonViewModel : NotifyPropertyChanged
{
    private string _firstName;
    private string _lastName;
    private int _age;

    public PersonViewModel()
    {
        ValidateCommand = new ActionCommand(() => Validate());
    }

    public string FirstName
    {
        get { return _firstName; }
        set { this.SetProperty(ref _firstName, value); }
    }

    public string LastName
    {
        get { return _lastName; }
        set { this.SetProperty(ref _lastName, value); }
    }

    public int Age
    {
        get { return _age; }
        set { this.SetProperty(ref _age, value); }
    }

    public ICommand ValidateCommand { get; set; }

    private void Validation()
    {
        // roll your own validation
    }
}
```

So the setters are simplified using the extension method SetProperty - the NotifyPropertyChanged implementation
doesn't handle anything to do with setting properties but just offers us the change event notifications etc. 

What you then generally find is that you might wish to extend this code to add things like
data error info. support for validation/error display, maybe ways to suppress property changes
using a BeginUpdate/EndUpdate pattern, i.e. some code might make a lot of changes to the view 
model but you only want to get the updates when EndUpdate is called. This is where the 
ExtendedNotifyPropertyChanged class comes in. It builds upon the NotifyPropertyChanged (as the
name suggests) supporting validation, error handling and more. Yet, at the same time it's still
a fairly lightweight class.

Next we have the ViewModelCommon class which derives from the ExtendedNotifyPropertyChanged. This
class is not really mean't for you to derive directly from but instead offers common functionality
for the heavier weight ViewModel classes. This code includes change tracking, revert and the likes 
along with the ability to use attributes in a sort of AOP-like way within your view model. So in scenarios
where your view model has more complicated requirements, this class implements those things, but 
at the cost of performances. However if you're displaying an input UI then the performance drop off when creating 
100,000's view models and handling 100,000's changes is probably not the number one concern. 

The ViewModelCommon is the base for ViewModel, ViewModelWithModel and ViewModelWithoutBacking.

Each of these adds what's required for their specific implementations. 

Once we start using the ViewModelCommon class, we get a lot of capabilities thrown in, with the sole aim
of reducing the plumbing code to a minimum and automating certain common functionality. Such common functionality 
can be set up using the attribute meta data. Such as data annotations for validation. 

*Possibly the best was to really achieve this ViewModel code, would be using some form of code weaver/injection/AOP,
but they come with their own possible issues.*

Let's look in a little more depth at the ViewModelCommon implementations and some of the features
we've mentioned.

# ViewModel

The ViewModel removes the need for backing fields, everything can be stored within the view model itself by
simply using the GetProperty/SetProperty combination in the property getter/setter. This reduces the "plumbing" code
to the barest minimum and makes for a clean and minimal design. Behind the scenes the data is stored within a Dictionary.

The downside of this implementation is probably obvious, by using a Dictionary, each property lookup is slower than 
a direct call to a backing field. 

Here's an example of what a ViewModel implemenation might look like

```
public class PersonViewModel : ViewModel
{
    public PersonViewModel()
    {
        ValidateCommand = new ActionCommand(() => Validate());
    }

    [Required(ErrorMessage = "First name is required")]
    public string FirstName
    {
        get { return GetProperty<string>(); }
        set { SetProperty(value); }
    }

    [Required(ErrorMessage = "Last name is required")]
    public string LastName
    {
        get { return GetProperty<string>(); }
        set { SetProperty(value); }
    }

    [Required]
    [Range(16, Int32.MaxValue, ErrorMessage = "Person must be older than 16")]
    public int Age
    {
        get { return GetProperty<int>(); }
        set { SetProperty(value); }
    }

    public ICommand ValidateCommand { get; set; }
}
```

Now the PersonViewModel is very basic example, but hopefully its obvious that it's also fairly powerful. 

The SetProperty/GetProperty combination is (sort of) similar to the way a DependencyObject works in WPF. 

# ViewModelWithoutBacking

*I'm not mad on the name of this class, so it might change in the future*

The ViewModel offers us a great way to achieve clean code of the fundamental view model like functionality, 
properties, validation etc. But, as stated, it's using a Dictionary lookup which ultimately has a performance
(however small) price. In such situations where you still require all the goodies of ViewModelCommon but want 
to handle the backing fields yourself, then you can derive from ViewModelWithoutBacking, like so 

```
public class PersonViewModel : ViewModel
{
    private string _firstName;
    private string _lastName;
    private int _age;

    public PersonViewModel()
    {
        ValidateCommand = new ActionCommand(() => Validate());
    }

    [Required(ErrorMessage = "First name is required")]
    public string FirstName
    {
        get { return GetProperty<string>(ref _firstName); }
        set { SetProperty(ref _firstName, value); }
    }

    [Required(ErrorMessage = "Last name is required")]
    public string LastName
    {
        get { return GetProperty<string>(ref _lastName); }
        set { SetProperty(ref _lastName, value); }
    }

    [Required]
    [Range(16, Int32.MaxValue, ErrorMessage = "Person must be older than 16")]
    public int Age
    {
        get { return GetProperty<int>(ref _age); }
        set { SetProperty(ref _age, value); }
    }

    public ICommand ValidateCommand { get; set; }
}
```

You still need to use the GetProperty method to allow the property to be defaulted etc. 

# ViewModelWithModel

If you already have a model with properties which map to the UI, maybe including it's own validation etc. and you need to
map this to a UI but you cannot implement INotifyPropertyChange or simply put, you don't want to change the code to be UI
friendly, you can derive a view model from ViewModelWithModel and with the GetProperty/SetProperty, delegate the set/get 
to the underlying model.

# GetProperty/SetProperty/ReadOnlyProperty

Each view model class - from the NotifyPropertyChanged extension methods through to the ViewModel/ViewModelWithoutBacking/ViewModelWithModel
stick to the premise of using a SetProperty method to see if a change is going to be made, if so raise a property changing event
then assign the change then raise a property changed event. In the case of the ViewModelCommon classes a GetProperty method is
used even when the developer provides the backing store, so that defaulting etc. can take place. The GetProperty/SetProperty are 
lazily initialized i.e. no value exists until one of the other is called. 

These two methods are probably fairly obvious, but a third, the ReadOnlyProperty exists - at first it might seem odd to have 
a ReadOnlyProperty when you could just have a getter using a GetProperty, but the ReadOnlyProperty is really a property composed
of other properties (or not, although this would be a little wasteful using this method).

Here's an example

```
public string FullName
{
    get { return ReadOnlyProperty(() => $"{FirstName} {LastName}"); }
}
```

Assuming that FirstName and LastName are properties using GetProperty syntax this ReadOnlyProperty will automatically update 
and raise a property change event on itself when FirstName or LastName properties change. FullName is dependent upon the other
properties and monitors their changes. In convential view model code, you'd change FirstName (for example), raise it's property
change event then raise a property change event for FullName to get the UI to update. This all happens automatically within
the view model virtue of the ReadOnlyProperty.

We can extend this further so that FullName could actually use "if" statements etc. to return data, if those "if" statements use
a property then, again, this code will raise a FullName property changed event and update the UI - basically what happens is each
time the ReadOnlyProperty is evaluated it builds a list of dependencies. When those dependencies change it changes and 
re-evaluates the dependencies again ready for the next dependency change.

# ViewModelRegistry

Before we start looking at the various attributes etc. I thought it worth pointing out that the ViewModelCommon class uses a
singleton ViewModelRegistry class. It's probably obvious that to get meta data/attributes such as default values, create instances etc.
we will need to use reflection to find what each property supports. As such we don't really want to do this for every 
instance of a PersonViewModel (for example), so in situations where we're creating many view models of the same type, the 
ViewModelRegistry will cache the property definitions as well as supply code to automatically create our default values, instances
etc. Comparers will also be reused, so if multiple properties across multiples types use the same comparer then only a single 
instance will be created. 

The only issue with this is I didn't want a situation where such property definitions were created and held in memory until the
application closes, so ViewModelCommon will register itself with the ViewModelRegistry to initiate creation of the propery 
definitions but must also unregister itself when being disposed of to hopefully, eventually, allow the property definitions to 
be cleaned up. 

*This area may need further tweaking as it's tested further, both from memory usage but probably more importantly a performance
persective.*

# Defaulting/Initializing properties

Like the old WinForms component model, you can use BeginInit/EndInit so that if you need to default
properties you can do so without property change events firing or change tracking kicking in. 

*You can use the InitializationDisposable class with a using clause to try to ensure you have a
matching number of BeginInit/EndInit combinations, otherwise by not calling EndInit enough the 
view model will be stuck in initialization mode.* 

Alternatively you can also use the DefaultValue attribute. For example, applying

```
[DefaultValue(16)]
```

to the Age property would set the age to 16 and like BeginInit/EndInit, this will not cause property
change events to occur as it happens when the view model initializes. 

We can take this concept further, let's say you have a property which takes an array of strings, so maybe a set of options
for a combobox. Now we can define the property like this 

```
[DefaultValue(new[] {"A", "B", "C"})]
public string[] Array
{
    get { return GetProperty<string[]>(); }
    set { SetProperty(value); }
}
```

Whilst the DefaultValue can be assigned to primitive types and arrays, what if we have another type, such as a view model
or ObservableCollection. In such a case we can use the DefaultValue in combination with the CreateInstance (or CreateInstanceUsing)
so that when the type is created, as long as it has a constructor which can take the DefaultValue argument then we can default
more complex, types. 

For example, this property will create an ExtendedObservableCollection, passing the DefaultValue
into the ctor.

```
[DefaultValue(new[] { "X", "Y", "Z"})]
[CreateInstance]
public ExtendedObservableCollection<string> Collection
{
    get { return GetProperty<ExtendedObservableCollection<string>>(); }
}
```

# Updating properties without event storms

Occasionally we get into a situation where we might wish to set many properties and hence we'll get many
change events (an event storm). In such situations its useful to defer these change events until our setting 
of the properties has completed and then fire a single event per property change as opposed to possible duplicates.
This is where the BeginUpdate/EndUpdate combination come in - we can suspend property changes until we're ready 
using BeginUpdate and then use EndUpdate to resume property change events. After EndUpdate, the deferred events are
published with duplicates removed.

*You can use the UpdatingDisposable class with a using clause to try to ensure you have a
matching number of BeginUpdate/EndUpdate combinations, otherwise by not calling EndUpdate enough the 
view model will be stuck in update mode.* 

# CreateInstance attribute

In situations where DefaultValue is not sufficiant (basically for anything other than basic types). This 
attribute can be applied to a property and will set up the property to a new instance of the property type. 

*This is particularily useful for defaulting collections or other view models, but requires a default
constructor. As mentioned in DefaultValue, combining with DefaultValue will try to pass your DefaultValue into the ctor
of the created instance.*

An example is the following, which doesn't require a setter but will automatically create an instance
of the collection (obviously include a setter if you might change the actual instance of the ObservableCollection)

```
[CreateInstance]
public ObservableCollection<PersonViewModel> Children => GetProperty<ObservableCollection<PersonViewModel>>();
```

# Comparer attribute (override the default comparison)

By default the EqualityComparer.Default for the property type is used to determine whether a property
has changed or not. In some circumstances you might wish to override this behavior with your own 
comparison code. 

Using the 

```
[Comparer(typeof(MyComparer))]
```

attribute on your property will cause an instance of MyComparer (requires default ctor) to be created
and where appropriate, this instance will be shared by all view models that use it - hence, we do not
end up creating 100's of instances of a comparer.

# Validation

Validation can be automatically handled if you use the data annotation ValidationAttribute 
classes. After a value changes, validation will occur (if it exists for the property). If the 
change fails validation, the error information will be updated and events fired so that UI elements
that handle data error info. will automatically update.

You can easily write your own custom validation attributes compatible with the data annotations, 
but if you prefer, you can also supply a Func<T, ValidationResult> to the SetProperty method and this
will be run during validation. If any single validation fails no further validation will take place on the
property until it's changed again. Whilst this does mean you will not see a list of all possible failures, 
it's obviously more performant than going through every validation step. 

You can also apply a validation function to the SetProperty method if you prefer. If writing your own 
validation function then the result should be a ValidationResult, but there's also code
to convert a boolean result to a ValidationResult with a supplied error message. For legacy code where you 
might be interacting with the view model's data error info. in your own code, you can use the 
Validation.Ignore method to return a null validation result (or ofcourse do this yourself).

# Change tracking

IRevertibleChangeTracking is supported. This is basically an interface for transactional sorts of 
functionality which we're using on the view model. i.e. You initialize your default values, then you make changes to 
them. Properties which are participating in change tracking (you can exclude them using the [ChangeTracking(false)]
attribute) will cause the view model to go "dirty" or in change tracking parlance, IsChanged 
will become true. The IRevertibleChangeTracking allows us to revert changes (this would be across 
the whole view model) or commit the changes so they become the new "default".

Removing a property from change tracking may be required if the property is something internal to 
your view model, for example if you support an IsEnabled property, you probably do not want to track its changes. 

By having change tracking, we're also able to handle Undo/Redo type functionality.

# Rule attributes

Rules can be created against view model properties which work a little like data annotation attributes, in that,
when a property changing event occurs a rule's PreInvoke is called and after a change occurs the PostInvoke is 
called. This allows you to write rules for your properties. One included rule is the PropertyChainAttribute which
can be applied to a property, when a change occurs it will automatically raise property change events for the 
properties listed in the PropertyChainAttribute. This can be useful in situations where, maybe a "Filter" property
is changed and we change some ICollectionView filter (for example) - hence we can chain things to get the UI to 
refresh the ICollectionView property after the Filter changes.

You might write your own rules that automatically change a child object based upon a parent property change etc.

*Rules are a bit of a legacy feature from a previous implementation of the view model classes. I would like to come 
up with "better" ways to do these, but for now they'll suffice.*


# Collection properties

When a property is set which supports INotifyCollectionChanged, INotifyPropertyChanged or the IItemChanged (supplied
as part of this project and used by ExtendedObservableCollection) changes can be "partially" tracked. At this time
a change simply causes the IsChanged flag to go true. Hence no undo currently exists on this code.

# Child view models

If a property supports INotifyPropertyChanged it will be change tracked by default, hence a change on the property 
will cause the IsChanged to go true, currently undo is not support on child objects.

# IDisposable on the view model

By default IDisposable is implemented on the view model base. This handles Unregistering the view model
type (the ctor Registers it). This process of register/unregister is used to reduce duplication of state. 
For example each PersonViewModel type will have the same properties (unless you're creating dynamic 
properties which are not supported here) and the attributes like-wise do not change. Hence we do not
want to reflect across this data and create multiple instances of comparers (for example) for every
instance of the PersonViewModel. Instead, when the first instance is created it registers itself with the
ViewModelRegistry which then creates a reusable state for this type. Subsequent creations of the 
PersonViewModel will register also, but each new instance will reuse the original state. Now we need a
way to eventually clear out the satte when not longer required, hence you can call Dispose directly or
let the finalizer do this. Unregister is called when no further types appear to reference the 
PersonViewModel state in the ViewModelRegistry and that state will be removed. If you do not Dispose/Unregister it 
just means there's the potential of unused state sitting around for the life of the application.

If your derived class needs to also handle Dispose, you should override DisposeManaged for managed resources and/or
DisposeUnManaged for unmanaged resources - this is designed along the guidelines of the IDisposable pattern.

# ICommand properties

By default, ICommand properties which are participating in the view model, i.e. have a SetProperty, will
not be change tracked. Generally speaking commands are not strictly part of the view model data. You can
override this by applying a ChangeTracking attribute with value True. By default non-ICommand properties are tracked, 
hence the the opposite, properties are changed tracked unless ChangeTracking is set to False;

*Usually we'd probably not include ICommand code in the view model using GetProperty/SetProperty.*

# ActionCommand

The name for this command is already used in Expressions Interactivity library, hence the name may 
change in the future if deemed too confusing.

The ActionCommand and ActionCommand<T> allow us to implement an ICommand object easily, passing 
execution and can execute to supplied functions. Unlike the *Expressions Interactivity* implementation 
this command allows us to declare the Execute and CanExecute methods after creation using a default
constructor. This allows the ActionCommand to be used via CreateInstance and CreateInstanceUsing attributes and
for the view model to assign methods to it post creation. This is really more a "nicety" to remove
the code to create such commands.

# AsyncCommand

The AsyncCommand and AsyncCommand<T> can be used with async/task based execute and can execute methods.
This allows us to execute on another thread and the command will set it's IsBusy property accordingly.

[![Uses NDepend]](https://user-images.githubusercontent.com/7886450/27169250-ee46df74-51a0-11e7-9e28-0732712cebdd.jpg, "Uses NDepend")
