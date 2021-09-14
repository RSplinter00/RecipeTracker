# Recipe Tracker
Recipe tracker is a mobile application which allows users to track their favourite bbq recipes.
The goal of this app is to make it easier for pitmasters to save and find their bbq recipes.

The app is currently still in progress. It is made using the frameworks [Xamarin.Forms](https://dotnet.microsoft.com/apps/xamarin/xamarin-forms) and [Prism Library](https://prismlibrary.com/docs/xamarin-forms/Getting-Started.html).  
Once released, the application will be available on Android devices and can be downloaded from the Play Store.

## Content
* [RecipeTracker](/RecipeTracker/RecipeTracker): Shared code between all operation systems.
  - [Controls](/RecipeTracker/RecipeTracker/Controls): The controls are derived from Xamarin.Forms' base controls, with custom functionalities.
  - [Converters](/RecipeTracker/RecipeTracker/Converters): The converters convert data from the viewmodel to make them readable for control properties in views.
  - [Models](/RecipeTracker/RecipeTracker/Models): The models are the blueprints for the apps data.
  - [Services](/RecipeTracker/RecipeTracker/Services): The services are responsible for communicating between viewmodels, views and the internet.
  - [ViewModels](/RecipeTracker/RecipeTracker/ViewModels): The view models of the corresponding views and is responsible for all the functionality behind the scene. 
  - [Views](/RecipeTracker/RecipeTracker/Views): The views of the pages with which the user interacts.
* [RecipeTracker.Android](/RecipeTracker/RecipeTracker.Android): Code for all Android specific features and allows the app to run on Android devices.
* [RecipeTracker.iOS](/RecipeTracker/RecipeTracker.iOS): Code for all iOS specific features and allows the app to run on iOS devices (**discontinued**).
* [RecipeTracker.UWP](/RecipeTracker/RecipeTracker.UWP): Code for all UWP specific features and allows the app to run on UWP devices (**discontinued**).
* [RecipeTracker.Unit](/RecipeTracker/RecipeTracker.Unit): Unit tests to test the integrity of the shared code.

## Key Features
* Sign in with a Google account
* Simple overview of your recipes
* Create and edit your recipes on your mobile device
* Save your recipes directly to the cloud
* Cache recipes when you don't have an internet connection

## ToDo:
For the ToDo list, see the [Trello board](https://trello.com/b/xGHzrPzV/todo).