#This is very much in an alpha/design phase

# Presentation.Core
Library of UI core functionality, such as MVVM implementation etc.

#Summary
The idea is to implement basic through to more complete capabilities for MVVM code.

#Aims/Requirements
Not in any specific order, the aim/requirements for this library will be as follows

1. Simple clean view models
2. Ability to have property changes trigger chained property changes in a declarative way
3. Allow for view models to work with backing fields, external models or backing storage
4. Easy validation capabilities
5. Use the DataAnnotation validation capabilities
6. Allow for interception of changes and changed properties to allow injection of rules
7. Ability to initialize the view model (i.e. not trigger changes)
8. Ability to pause/resuxe view model changes (i.e. Begin/EndUpdate)
9. Support latest .NET 4.6/C# 6 syntax as well as compilable to .NET 4.0 (to support my legacy code)
10. Removal of magic strings - whilst we will need to support strings, I want to remove magics strings 
    or at least implement code to use alternatives going forward (use CallMemberName for C# 6)
11. Support simply low level capabilities of MVVM, i.e. allow the user to handle code at a low level 
    and also support capabilities where the base class does most of the work, so allowing pretty 
    much any usage style   