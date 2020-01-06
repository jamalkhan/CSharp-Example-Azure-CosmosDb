Source: 
https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet

Quickstart: Build a .NET web app using Azure Cosmos DB's API for MongoDB
========================================================================

-   05/20/2019
-   5 minutes to read
-   [

    -   ![](https://github.com/markjbrown.png?size=32)

    -   ![](https://github.com/SnehaGunda.png?size=32)

    -   ![](https://github.com/rimman.png?size=32)

    -   ![](https://github.com/DennisLee-DennisLee.png?size=32)

    -   ![](https://github.com/changeworld.png?size=32)

    -   +7

    ](https://github.com/Microsoft/azure-docs/blob/master/articles/cosmos-db/create-mongodb-dotnet.md "12 Contributors")

.NET

Azure Cosmos DB is Microsoft's globally distributed multi-model database service. You can quickly create and query document, key/value and graph databases, all of which benefit from the global distribution and horizontal scale capabilities at the core of Cosmos DB.

This quickstart demonstrates how to create a Cosmos account with [Azure Cosmos DB's API for MongoDB](https://docs.microsoft.com/en-us/azure/cosmos-db/mongodb-introduction). You'll then build and deploy a tasks list web app built using the [MongoDB .NET driver](https://docs.mongodb.com/ecosystem/drivers/csharp/).

Prerequisites to run the sample app[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#prerequisites-to-run-the-sample-app)
-------------------------------------------------------------------------------------------------------------------------------------------------

To run the sample, you'll need [Visual Studio](https://www.visualstudio.com/downloads/) and a valid Azure Cosmos DB account.

If you don't already have Visual Studio, download [Visual Studio 2019 Community Edition](https://www.visualstudio.com/downloads/) with the ASP.NET and web development workload installed with setup.

If you don't have an [Azure subscription](https://docs.microsoft.com/en-us/azure/guides/developer/azure-developer-guide#understanding-accounts-subscriptions-and-billing), create a [free account](https://azure.microsoft.com/free/?ref=microsoft.com&utm_source=microsoft.com&utm_medium=docs&utm_campaign=visualstudio) before you begin.

Create a database account[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#create-a-database-account)
-----------------------------------------------------------------------------------------------------------------------------

1.  In a new window, sign in to the [Azure portal](https://portal.azure.com/).

2.  In the left menu, select Create a resource, select Databases, and then under Azure Cosmos DB, select Create.

    ![Screenshot of the Azure portal, highlighting More Services, and Azure Cosmos DB](https://docs.microsoft.com/en-us/azure/includes/media/cosmos-db-create-dbaccount-mongodb/create-nosql-db-databases-json-tutorial-1.png)

3.  In the Create Azure Cosmos DB Account page, enter the settings for the new Azure Cosmos DB account.

    | Setting | Value | Description |
    | --- | --- | --- |
    | Subscription | Your subscription | Select the Azure subscription that you want to use for this Azure Cosmos DB account. |
    | Resource Group | Create new

    Then enter the same unique name as provided in ID | Select Create new. Then enter a new resource-group name for your account. For simplicity, use the same name as your ID. |
    | Account Name | Enter a unique name | Enter a unique name to identify your Azure Cosmos DB account. Because *documents.azure.com* is appended to the ID that you provide to create your URI, use a unique ID.

    The ID can use only lowercase letters, numbers, and the hyphen (-) character. It must be between 3 and 31 characters in length. |
    | API | Azure Cosmos DB's API for MongoDB | The API determines the type of account to create. Azure Cosmos DB provides five APIs: Core (SQL) for document databases, Gremlin for graph databases, Azure Cosmos DB's API MongoDB for document databases, Azure Table, and Cassandra. Currently, you must create a separate account for each API.

    Select MongoDB because in this quickstart you are creating a table that works with the MongoDB. |
    | Location | Select the region closest to your users | Select a geographic location to host your Azure Cosmos DB account. Use the location that's closest to your users to give them the fastest access to the data. |

    Select Review+Create. You can skip the Network and Tags section.

    ![The new account page for Azure Cosmos DB](https://docs.microsoft.com/en-us/azure/includes/media/cosmos-db-create-dbaccount-mongodb/azure-cosmos-db-create-new-account.png)

4.  The account creation takes a few minutes. Wait for the portal to display the Congratulations! Your Cosmos account with wire protocol compatibility for MongoDB is ready page.

    ![The Azure portal Notifications pane](https://docs.microsoft.com/en-us/azure/includes/media/cosmos-db-create-dbaccount-mongodb/azure-cosmos-db-account-created.png)

The sample described in this article is compatible with MongoDB.Driver version 2.6.1.

Clone the sample app[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#clone-the-sample-app)
-------------------------------------------------------------------------------------------------------------------

First, download the sample app from GitHub.

1.  Open a command prompt, create a new folder named git-samples, then close the command prompt.

    bashCopy

    ```
    md "C:\git-samples"

    ```

2.  Open a git terminal window, such as git bash, and use the `cd` command to change to the new folder to install the sample app.

    bashCopy

    ```
    cd "C:\git-samples"

    ```

3.  Run the following command to clone the sample repository. This command creates a copy of the sample app on your computer.

    bashCopy

    ```
    git clone https://github.com/Azure-Samples/azure-cosmos-db-mongodb-dotnet-getting-started.git

    ```

If you don't wish to use git, you can also [download the project as a ZIP file](https://github.com/Azure-Samples/azure-cosmos-db-mongodb-dotnet-getting-started/archive/master.zip).

Review the code[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#review-the-code)
---------------------------------------------------------------------------------------------------------

This step is optional. If you're interested in learning how the database resources are created in the code, you can review the following snippets. Otherwise, you can skip ahead to [Update your connection string](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#update-your-connection-string).

The following snippets are all taken from the Dal.cs file in the DAL directory.

-   Initialize the client.

    C#Copy

    ```
        MongoClientSettings settings = new MongoClientSettings();
        settings.Server = new MongoServerAddress(host, 10255);
        settings.UseSsl = true;
        settings.SslSettings = new SslSettings();
        settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;

        MongoIdentity identity = new MongoInternalIdentity(dbName, userName);
        MongoIdentityEvidence evidence = new PasswordEvidence(password);

        settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

        MongoClient client = new MongoClient(settings);

    ```

-   Retrieve the database and the collection.

    C#Copy

    ```
    private string dbName = "Tasks";
    private string collectionName = "TasksList";

    var database = client.GetDatabase(dbName);
    var todoTaskCollection = database.GetCollection<MyTask>(collectionName);

    ```

-   Retrieve all documents.

    C#Copy

    ```
    collection.Find(new BsonDocument()).ToList();

    ```

Create a task and insert it into the collection

C#Copy

```
 public void CreateTask(MyTask task)
 {
     var collection = GetTasksCollectionForEdit();
     try
     {
         collection.InsertOne(task);
     }
     catch (MongoCommandException ex)
     {
         string msg = ex.Message;
     }
 }

```

Similarly, you can update and delete documents by using the [collection.UpdateOne()](https://docs.mongodb.com/stitch/mongodb/actions/collection.updateOne/index.html) and [collection.DeleteOne()](https://docs.mongodb.com/stitch/mongodb/actions/collection.deleteOne/index.html) methods.

Update your connection string[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#update-your-connection-string)
-------------------------------------------------------------------------------------------------------------------------------------

Now go back to the Azure portal to get your connection string information and copy it into the app.

1.  In the [Azure portal](https://portal.azure.com/), in your Cosmos account, in the left navigation click Connection String, and then click Read-write Keys. You'll use the copy buttons on the right side of the screen to copy the Username, Password, and Host into the Dal.cs file in the next step.

2.  Open the Dal.cs file in the DAL directory.

3.  Copy your username value from the portal (using the copy button) and make it the value of the username in your Dal.cs file.

4.  Then copy your host value from the portal and make it the value of the host in your Dal.cs file.

5.  Finally copy your password value from the portal and make it the value of the password in your Dal.cs file.

You've now updated your app with all the info it needs to communicate with Cosmos DB.

Run the web app[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#run-the-web-app)
---------------------------------------------------------------------------------------------------------

1.  In Visual Studio, right-click on the project in Solution Explorer and then click Manage NuGet Packages.

2.  In the NuGet Browse box, type *MongoDB.Driver*.

3.  From the results, install the MongoDB.Driver library. This installs the MongoDB.Driver package as well as all dependencies.

4.  Click CTRL + F5 to run the application. Your app displays in your browser.

5.  Click Create in the browser and create a few new tasks in your task list app.

Review SLAs in the Azure portal[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#review-slas-in-the-azure-portal)
-----------------------------------------------------------------------------------------------------------------------------------------

The Azure portal monitors your Cosmos DB account throughput, storage, availability, latency, and consistency. Charts for metrics associated with an [Azure Cosmos DB Service Level Agreement (SLA)](https://azure.microsoft.com/support/legal/sla/cosmos-db/) show the SLA value compared to actual performance. This suite of metrics makes monitoring your SLAs transparent.

To review metrics and SLAs:

1.  Select Metrics in your Cosmos DB account's navigation menu.

2.  Select a tab such as Latency, and select a timeframe on the right. Compare the Actual and SLA lines on the charts.

    ![Azure Cosmos DB metrics suite](https://docs.microsoft.com/en-us/azure/includes/media/cosmos-db-tutorial-review-slas/azure-cosmosdb-metrics-suite.png)

3.  Review the metrics on the other tabs.

Clean up resources[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#clean-up-resources)
---------------------------------------------------------------------------------------------------------------

When you're done with your web app and Azure Cosmos DB account, you can delete the Azure resources you created so you don't incur more charges. To delete the resources:

1.  In the Azure portal, select Resource groups on the far left. If the left menu is collapsed, select ![Expand button](https://docs.microsoft.com/en-us/azure/includes/media/cosmos-db-delete-resource-group/expand.png) to expand it.

2.  Select the resource group you created for this quickstart.

    ![Select the resource group to delete](https://docs.microsoft.com/en-us/azure/includes/media/cosmos-db-delete-resource-group/delete-resources-select.png)

3.  In the new window, select Delete resource group.

    ![Delete the resource group](https://docs.microsoft.com/en-us/azure/includes/media/cosmos-db-delete-resource-group/delete-resources.png)

4.  In the next window, enter the name of the resource group to delete, and then select Delete.

Next steps[](https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#next-steps)
-----------------------------------------------------------------------------------------------

In this quickstart, you've learned how to create a Cosmos account, create a collection and run a console app. You can now import additional data to your Cosmos database.