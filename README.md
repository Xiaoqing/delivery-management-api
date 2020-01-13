# delivery-management-api
### Technical Details
1. The solution is built on .NET Core 3.1 and Visual Studio 2019
2. To run the solution, either open it in VS2019 or run commmand 'docker-compose up' at the solution root
3. The API will be availbe at https://localhost:5001
4. The Swagger documentation is available at https://localhost:5001/swagger, which will show some details of each endpoint in the API. However it is not a polished documentation and a lot more yet to be done
5. The API is protected with IdentityServer. Before calling any endpoint, get a valid access token from https://demo.identityserver.io/connect/token using the following details:  
  client_id:client  
  client_secret:secret  
  grant_type:client_credentials  
  The API currently only support client_credentials flow.
  
  
6. Logging is done with Serilog and supports request logging and correlationid. The log file will be located in the solution root folder /logs
7. Testing is done with NUnit. Due to time constraints, I only managed to write some unit tests for the controller
8. Data storage - for simplicity I have used an in memory data storage
9. I have added input validation using Fluent, which I think is very important for a public API

### Not Implemented
1. The delivery expired requirement is not implemented. Possible solutions:
   * Use a scheduled service to periodically check all deliveries not completed or cancelled and mark them expired if end time has elapsed. But this may not be scalable enough
   * Use a queue solution which supports delayed delivery. When a new delivery is created, adds it to the queue and set the delivery time to the end time. Then build a consumer for the queue to check the delivery status. This solution is going to be more scalable and put less burden on storage
   
1. Partner facing Pub/Sub API for receiving state updates.
   * To be able to scalable enough to support large number of partners, i would go down the webhook approach, e.g. each partners need to register a webhook in order to receive status update
   * On the API side, publish an event from the domain service whenever there is an status change. These status change events will be published to a queue or streaming solution (such as Kafka) and a consumer (or consumer group) will consume these events and call the corresponding webhook to send the status change to partners
