# Move Like Jogger

A web application that allows you to log your moves!

### Prerequisities

* .NET Framework 4.6.1+
* MS SQL Server 2008+


## Technologies and Frameworks used

* MS SQL
* ASP.NET MVC 5 with OWIN
* OData v4
* StructureMap
* AngularJS
* jQuery
* Bootsrap

## Brief RESTful API description

The back-end exposes a RESTful API on '/api' path:

### Entity Sets

* **/api/users** - User Management endpoint, only *Admin* and *Manager* authorized
* **/api/moves** - User Moves data, available for correcponding user or Admin only

**Invoking Entity Set endpoints**
GET supports OData query filter, for instance: */api/moves?$count=true&$orderby=Date desc&$top=10&$filter=UserId eq 'a7e85fe7-167c-4c3e-8747-fb2c20d8895c'*
POST, PUT and DELETE require *key* parameter according to OData guidelines: */api/users('a7e85fe7-167c-4c3e-8747-fb2c20d8895c')* or */api/moves(1337)*

### Unbound functions and actions

Functions:

* GET **/api/identity** - returns current user info
* GET **/api/statistics** - returns Moves Statistics, has optional *userId* and *daysBeforeToday* parameters

Actions:

* POST **/api/login** - authentication endpoint
* POST **/api/logout** - authentication endpoint
* POST **/api/register** - authentication endpoint
* PUT  **/api/account** - authentication endpoint, update account details

### Seed data

Initially the application has only default **admin** user with a default password **iadmin**.
There are **Admin** and **Manager** roles which can be assigned to any account by anyone with **Admin** role.
All the users without any role assigned are considered as *Ordinary Users*.

## User Experience

### General features

You can create an Account, log in and update your Password and/or Email.

There are three types of Account:
* Ordinary User
* User Manager
* Admin


Only *Ordinary Users* are able to log their Moves data.
*User Manager* and *Admin* are technical accounts and do not participate in Moves tracking unless their role is removed and they become an *Ordinary Users*.

### Ordinary User

You can see, add, edit and delete your Moves data.
You can filter Moves data by a date range.
You can revert a deleted Move record if you didn't switch to another screen or page or apply filter.
A detailed Statistics for the last 7 days is also available to you if you have posted any Moves in those days.

### User Manager

You're the one who can manage Users: change their User Name, Email and/or password.
You can not make any changes to other Managers or Admins.

### Admin

You have the same features as User Manager, but you can manage all users, including other Admins and User Managers, and you can also assign these privileges to any Ordinary User.
Beside that you're able to manage any Ordinary User's Moves data, including restore of previously deleted records.

## Authors

* **[Dmitry Duka](http://git.toptal.com/u/Dmitry-Duka)** - *Initial implementation*

## License restrictions, including 3rd party components

This project uses some 3rd party components with MIT License

