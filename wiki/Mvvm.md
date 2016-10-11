# View Models

## The MVVM (Model view, view model) pattern

MVVM is (simply referred to from now on as the view model or VM) used a lot in  WPF/UWP 
applications as well as the likes of Xamarin Forms etc. and relies heavily on the idea 
of data binding. Every time we write our VM code we need to implement a bunch of 
interfaces and depending upon our requirements, implement validation, error handling etc.

We've probably all written our own basic base classes to handle these features. The ViewModel 
is one such class which incorporates code to handle various ways of storing state as well as 
including the ability to handle validation and error information in various ways.

It's designed to not force a specific way of doing things but instead attempts to allow for
different usage patterns.

For example, we generally have three ways that our VM might store state, i.e. properties. The 
first is via backing fields. Private fields within the VM that we get and set to. Alternatively
we wish to use a model that is separate from the VM and have the VM get and set directly to the 
underlying model. An alternative to backing fields, which reduces the "clutter" within a VM, is 
to store the fields in some form of backing store.

The choice of which way to implement your VM, i.e. backing fields, domain model or backing store
is partly down to how you handle your data and which features you want to incorporate in your VM.
Each has pros and cons.

## Lightweight view models (the NotifyPropertyChanged class)

Before we get into using the ViewModel class to implement our view models. The VM library was 
designed to allow lightweight view model's i.e. you write most of the code and the base class
simply supplies the property changed code, hence a ViewModel derives from the NotifyPropertyChanged
class which literally just handles the property changed interfaces (INotifyPropertyChanged and 
INotifyPropertyChanging) and with the use of extension methods allows you to still write simple
code to handle setting of properties and property changes.

```csharp
public class MyViewModel : NotifyPropertyChanged
{
    private string name;

    public string Name
    {
        get { return name; }
        set { this.RaiseAndSetIfChanged(ref name, value); }
    }
}
```

## A fuller features view model (the ViewModel class)

The ViewModel class dervies from NotifyPropertyChanged and added the ability to use backing fields, 
domain model fields and backing stores. It also includes the ability to work with validation 
capabilities and error info. and more. If all you need to to handle property changes then the 
NotifyPropertyChanged class is all you need, but if you want to reused existing implementations of
rules, data error info. validation etc. then the ViewModel is the way to go.

The PropertyChanging and PropertyChanged calls within the ViewModel are hooked into the Rules 
system, so Rule objects can be executed (Rules currently include property chaining, validation and invoking 
functions/actions). Validation is also built into the ViewModel class and if a validation object is supplied 
will also run validation on property changed events using the supplied validation class. 

## Backing Fields

Using backing fields in our VM is a simple and fairly standard way of implementing VM's. We simply define
fields within our view model class and use the SetProperty method(s) to check whether a field has changed,
if so the PropertyChanging events are raises, then the value is assigned to the backing field and the 
PropertyChanged events raised.

These offer a fast, type safe mechanism for storing your VM state, but on larger VM's add clutter (which can
obviously be hidden using #region). There's a little more plumbing code and implementing more complicated 
functionality, such as transactions/revert code etc. can further clutter the view model implementations. But if 
you don't need such functionality and you don't need to go direct to an encapsulated model, then this is
the best solution.  

```csharp
public class MyViewModel : ViewModel
{
    private string name;

    public string Name
    {
        get { return name; }
        set { SetProperty(ref name, value); }
    }
} 
```

## Get/set against a given model

Another often used way of implementing view models, particularily when the underlying model is fairly complex
is to encapsulate a model into the VM and each get is invoked against the model and likewise property
setting is also directed through the the underlying model. 

This may fit in with the design of your model's and has the advantage of reusing a model's existing logic code.

```csharp
public class MyViewModel : ViewModel
{
    private readonly MyDomainObject domainObject = new MyDomainObject();

    public string Name
    {
        get { return domainObject.Name; }
        set 
        {
            SetProperty(() => domainObject.Name,
                newValue => domainObject.Name = newValue,
                value); 
        }
    }   
}
```

## Backing store

The backing store allows us to offload the storage of the backing fields to a storage mechanism (the
SimpleBackingStore is a very simple implementation which uses a Dictionary). This removes the clutter 
in cases where a VM has a large number of fields abd reduces the view model code to the bare minimmum 
at the expense of a slightly slower get/set mechanism, i.e. a dictionary and also potential boxing/unboxing. 
However this does also allow us to create more complex/reusable storage capabilities outside of our VM. 
Such capabilities might include revert/rollback functionality, transaction functionality, external 
persistance options etc.

```csharp
public class MyViewModel : ViewModel
{
    public MyViewModel() :
       base(new SimpleBackingStore(), new DataErrorInfo())
    {           
    }

    public string Name
    {
        get { return GetProperty<string>(); }
        set { SetProperty(value); }
    }
}
```