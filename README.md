# projectapi
## Descrption
### This project is a REST API which implements the functionality of a product catalog. There are two main entities: product catgeories and products. 
Products have a unique ID, name, desciption, category ID, and a list of product specifications (which are defined by the category).
Categories have a unique ID, name, desciption, and the list of product specifications for its products.
Also, these entities are auditable for creation and mofication (i.e. the dates of creation and last modification are provided) and are soft-deletable.
The application also supports batch upload functionality - the user can upload a json/csv file with either categories or products, and then 
check the status of the request by its ID. There is also logging to file for the whole project.
## Technical details
The application is built using [ASP.NET Core](https://github.com/dotnet/aspnetcore) Web API, 
[Entity Framework](https://github.com/dotnet/efcore) is used for database interaction, all HTTP requests are handled 
using [MediatR](https://github.com/jbogard/MediatR).
Operations of creation, modification and deletion, as well as the upload service, are all functioning 
through [MassTransit](https://github.com/MassTransit/MassTransit)
using RabbitMq server.
Validation of the models provided by the user is done using [FluentValidation](https://github.com/FluentValidation/FluentValidation), 
and the entity-to-model and model-to-entity mapping is done using [AutoMapper](https://github.com/AutoMapper/AutoMapper).
Also, some unit tests are provided for the CRUD operations, which use [xUnit](https://github.com/xunit/xunit) and [Moq](https://github.com/moq/moq).
