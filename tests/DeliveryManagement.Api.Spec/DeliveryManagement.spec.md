You work for a delivery company that takes orders from merchants and dispatches the orders using partners to users. The company decides to create a DeliveryManagement API that offers the following capabilities:
- View a delivery
- Create a delivery
- Approve a delivery
- Cancel a delivery
- Complete a delivery

A delivery can have 5 different states: created, approved, completed, cancelled, expired.  
Users may approve a delivery before it starts.  
Partner may complete a delivery, that is already in approved state.  
If a delivery is not completed during the access window period, then it should expire.   
Either the partner or the user should be able to cancel a pending delivery (in state created or approved).

### View a delivery
An existing delivery can be retrieved using the delivery_id. If the delivery_id provided is not associated with any delivery, an NotFound error will be returned.

The shape of the response for a delivery will be like this:
```
{
   "accessWindow":{
      "startTime":"2019-12-13T09:00:00Z",
      "endTime":"2019-12-13T11:00:00Z"
   },
   "recipient":{
      "name":"John Doe",
      "address":"Merchant Road, London",
      "email":"john.doe@mail.com",
      "phoneNumber":"+44123123123"
   },
   "order":{
      "orderNumber":"12209667",
      "sender":"Ikea"
   }
}
```

The endpoint for retrieving a delivery is ```/v1/delivery/{delivery_id}``` and the operation is ```GET```.

| View an existing delivery                                                           ||||
| #Description              | Delivery Id Exists | Response Status? | Response Code?     |
| ------------------------- | ------------------ | ---------------- | ------------------ |
| Delivery exists           | true               | Ok               |                    |
| Delivery doesn't exist    | false              | NotFound         | delivery_not_found |

### Create a delivery
when creating a delivery, we need to make sure all the required information is provided (see [DeliveryInputValidation](./DeliveryInputValidation.spec.md#validating-delivery) for more details). In addition, we need to make sure no duplicate delivery is created for the same order number unless the previous order has been cancelled or expired. 

Delivery can be created via endpoint ```/v1/delivery``` with a ```POST``` operation.

An example of the input for creating a delivery as below (it is also assigned to ```$delivery_request``` that can be referred to later):  
[{  
&nbsp;&nbsp;"accessWindow": {  
&nbsp;&nbsp;&nbsp;&nbsp;"startTime": "2019-12-13T09:00:00Z",  
&nbsp;&nbsp;&nbsp;&nbsp;"endTime": "2019-12-13T11:00:00Z"  
&nbsp;&nbsp;},  
&nbsp;&nbsp;"recipient": {  
&nbsp;&nbsp;&nbsp;&nbsp;"name": "John Doe",  
&nbsp;&nbsp;&nbsp;&nbsp;"address": "Merchant Road, London",  
&nbsp;&nbsp;&nbsp;&nbsp;"email": "john.doe@mail.com",  
&nbsp;&nbsp;&nbsp;&nbsp;"phoneNumber": "+44123123123"  
&nbsp;&nbsp;},  
&nbsp;&nbsp;"order": {  
&nbsp;&nbsp;&nbsp;&nbsp;"orderNumber": "12209667",  
&nbsp;&nbsp;&nbsp;&nbsp;"sender": "Ikea"  
&nbsp;&nbsp;}  
}](# "$delivery_request")

| Create a delivery                                                                                                                                                                               |||||||
| #Description                                                | Delivery Request  | Order Delivered Before? | Previous Delivery State? | Creation Allowed? | Response Status? | Response Error Code?    |
| ----------------------------------------------------------- | ----------------- | ----------------------- | ------------------------ | ----------------- | ---------------- | ----------------------- |
| Create a new delivery                                       | $delivery_request | false                   |                          | true              | Ok               |                         |
| Delivery for same order was created, creation not allowed   | $delivery_request | true                    | created                  | false             | Conflict         | order_already_delivered |
| Delivery for same order was approved, creation not allowed  | $delivery_request | true                    | approved                 | false             | Conflict         | order_already_delivered |
| Delivery for same order was completed, creation not allowed | $delivery_request | true                    | completed                | false             | Conflict         | order_already_delivered |
| Delivery for same order was cancelled, creation allowed     | $delivery_request | true                    | cancelled                | true              | Ok               |                         |
| Delivery for same order was expired, creation allowed       | $delivery_request | true                    | expired                  | true              | Ok               |                         |

### Approve a delivery
A delivery can be approved if:
- The delivery_id exists
- Its state is Created
- The delivery window hasn't started 

Approving an already approved delivery won't do anything and an Ok response will be returned.

The endpoint for approving a delivery is ```/v1/delivery/{delivery_id}/approve``` and the operation is ```PUT```.

| Approve a delivery                                                                                                                                                           |||||||
| #Description                                   | Delivery Id Exists | Current Delivery State | Windows Started | Approval Allowed? | Response Status? | Response Error Code?       |
| ---------------------------------------------- | ------------------ | ---------------------- | --------------- | ----------------- | ---------------- | -------------------------- |
| Delivery doesn't exist                         | false              |                        | false           | false             | NotFound         | delivery_not_found         |
| Delivery expired can't be approved             | true               | expired                | false           | false             | Conflict         | delivery_operation_invalid |
| Delivery completed can't be approved           | true               | completed              | false           | false             | Conflict         | delivery_operation_invalid |
| Delivery cancelled can't be approved           | true               | cancelled              | false           | false             | Conflict         | delivery_operation_invalid |
| Delivery just created can be approved          | true               | created                | false           | true              | Ok               |                            |
| Delivery with window started can't be approved | true               | created                | true            | false             | Conflict         | delivery_operation_invalid |
| Delivery approved can be approved              | true               | approved               | false           | true              | Ok               |                            |

### Cancel a delivery
A delivery can be cancelled if:
- The delivery_id exists
- Its state is in Created or Approved. 

Cancelling an already cancelled delivery won't do anything and an Ok response will be returned.

The endpoint for cancelling a delivery is ```/v1/delivery/{delivery_id}/cancel``` and the operation is ```PUT```.

An example of the input for creating a delivery as below(it is also assigned to ```$cancellation_request``` that can be referred to later):   
[{  
&nbsp;&nbsp;"reason": "order has been cancelled"  
}](# "$cancellation_request")

| Cancel a delivery                                                                                                                                                                 |||||||
| #Description                              | Cancellation Request  | Delivery Id Exists | Current Delivery State | Cancellation Allowed? | Response Status? | Response Error Code?       |
| ----------------------------------------- | --------------------- | ------------------ | ---------------------- | --------------------- | ---------------- | -------------------------- |
| Delivery doesn't exist                    | $cancellation_request | false              |                        | false                 | NotFound         | delivery_not_found         |
| Delivery expired can't be cancelled       | $cancellation_request | true               | expired                | false                 | Conflict         | delivery_operation_invalid |
| Delivery completed can't be cancelled     | $cancellation_request | true               | completed              | false                 | Conflict         | delivery_operation_invalid |
| Delivery cancelled can be cancelled again | $cancellation_request | true               | cancelled              | true                  | Ok               |                            |
| Delivery just created can be cancelled    | $cancellation_request | true               | created                | true                  | Ok               |                            |
| Delivery approved can be cancelled        | $cancellation_request | true               | approved               | true                  | Ok               |                            |

### Complete a delivery
A delivery can be completed if:
- The delivery_id exists
- Its state is Approved. 

Completing an already completed delivery won't do anything and an Ok response will be returned.

The endpoint for completing a delivery is ```/v1/delivery/{delivery_id}/complete``` and the operation is ```PUT```.

| Complete a delivery                                                                                                                                                     |||||||
| #Description                              | Delivery Id Exists | Current Delivery State | Approval Allowed? | Response Status? | Response Error Code?       |
| ----------------------------------------- | ------------------ | ---------------------- | ----------------- | ---------------- | -------------------------- |
| Delivery doesn't exist                    | false              |                        | false             | NotFound         | delivery_not_found         |
| Delivery expired can't be completed       | true               | expired                | false             | Conflict         | delivery_operation_invalid |
| Delivery just created can't be completed  | true               | completed              | false             | Conflict         | delivery_operation_invalid |
| Delivery cancelled can't be completed     | true               | cancelled              | false             | Conflict         | delivery_operation_invalid |
| Delivery approved can be completed        | true               | created                | true              | Ok               |                            |
| Delivery completed can be completed again | true               | approved               | true              | Ok               |                            |
