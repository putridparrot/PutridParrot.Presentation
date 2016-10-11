# Rules

## What are Rules used for?

One of the things I wanted to achieve is cleaner view model code specifically around the VM's
properties. 

In some cases we want to check if a property changed and raise a change on another property. 
For example, if we have a FirstName and LastName property (both of which are read/write) and
another property FullName which is a readonly property made up of the FirstName and LastName
then when FirstName or LastName change we need to raise a FullName property changed event. 

This is easy enough to do in our property setters for FirstName and LastName but we can also
allow the view model to do this automatically for us by "declaring" property chain rules. When
a FirstName or LastName change is seen in the VM it will automatically raise the FullName 
property changed event for us.

So this is one example of a rule. Basically rules are interception methods. When a property 
changing event occurs, a rule method is called. When the property changes another rule method
is called. This allows rules to intercept changing and changes in the VM.

## What rules can we use?

The currently implemented rules are the PropertyChainRule (as described above), the 
PropertyChangeRule which allows us to execute a function or action when a change occurs and 
the ValidationRule which allows us to define validation code to be executed on a property change.  

A rule is simple a classed derived from the Rule class, which implements a PreInvoke and/or 
PostInvoke method. These methods are called by the PropertyChanging and PropertyChanged 
functionality from the ViewModel.

## Things to watch out for

Obviously the more rules you define in your VM the slower the property changing and property changed 
events can be. These rules are not (at this time) asynchronous. Another things to beware of is
circular functionality, i.e. if your PropertyChainRule (for example) were to be applied to FirstName 
property changes and it in turn raised FullName which itself raised a FirstName change, then things 
will stackoverflow eventually. 