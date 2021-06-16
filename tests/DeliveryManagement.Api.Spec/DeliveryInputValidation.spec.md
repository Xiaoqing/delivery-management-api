When creating a delivery, we need to make sure all the required information are provided and valid.  

### Validating Delivery
A Delivery must have the following information present in the input; otherwise relevant error will be returned:
- Order
- Recipient 
- AccessWindow

| Delivery details must be valid                                                                                                  |||||
| #Description                   | Field        | Has Value | Is Valid? | Error Message?                                              |
| ------------------------------ | ------------ | --------- | --------- | ----------------------------------------------------------- |
| Order is not provided          | Order        | false     | false     | The order details for the delivery must be provided         |
| Order is provided              | Order        | true      | true      |                                                             |
| Recipient is not provided      | Recipient    | false     | false     | The recipient details for the delivery must be provided     |
| Recipient is provided          | Recipient    | true      | true      |                                                             |
| AccessWindow is not provided   | AccessWindow | false     | false     | The access window details for the delivery must be provided |
| AccessWindow is provided       | AccessWindow | true      | true      |                                                             |

### Validating AccessWindow
The access window must have a start & end time and the start time must be before the end time. Both start and end time must be in zulu format (e.g. 2019-12-13T11:00:00Z). More details about zulu time can be found at https://en.wikipedia.org/wiki/ISO_8601#Coordinated_Universal_Time_(UTC).   

| AccessWindow details must be valid                                                                                |||||
| #Description                       | Field     | Value                | Is Valid? | Error Message?                    |
| ---------------------------------- | --------- | -------------------- | --------- | --------------------------------- |
| StartTime is not provided          | StartTime |                      | false     | Start time must be provided       |
| StartTime is not well formatted    | StartTime | 2019-12-13 11:00:00  | false     | Start time must be in zulu format |
| Valid StartTime is provided        | StartTime | 2019-12-13T11:00:00Z | true      |                                   |
| EndTime is not provided            | EndTime   |                      | false     | End time must be provided         |
| EndTime is not well formatted      | EndTime   | 2019-12-13 11:00:00  | false     | End time must be in zulu format   |
| Valid EndTime is provided          | EndTime   | 2019-12-13T11:00:00Z | true      |                                   |


| AccessWindow must be a valid window                                                                                                  |||||
| #Description                        | StartTime           | EndTime             | Is Valid? | Error Message?                             |
| ----------------------------------- | ------------------- | ------------------- | --------- | ------------------------------------------ |
| StartTime must be before EndTime    | 2019-12-13 12:00:00 | 2019-12-13 11:00:00 | false     | Start time must be before end time         |
| The window must > 1 hour            | 2019-12-13 10:30:00 | 2019-12-13 11:00:00 | false     | The access window must be more than 1 hour |
| Valid window                        | 2019-12-13 11:00:00 | 2019-12-13 11:00:00 | true      |                                            |

### Validating Order
An Order must have the following information; otherwise relevant error will be returned:
- OrderNumber
- Sender

Both fields are interpreted as strings and can be any text.

| Order details must be valid                                                                        |||||
| #Description                | Field       | Value      | Is Valid? | Error Message?                    |
| --------------------------- | ----------- | ---------- | --------- | --------------------------------- |
| OrderNumber is not provided | OrderNumber |            | false     | The order number must be provided |
| OrderNumber is provided     | OrderNumber | order_123  | true      |                                   |
| Sender is not provided      | Sender      |            | false     | The sender must be provided       |
| Sender is provided          | Sender      | sender_123 | true      |                                   |

### Validating Recipient
A Recipient must have the following inforamtion; otherwise relevant error will be returned:
- Name
- Address
- Email
- PhoneNumber

Email address must be in a valid format. All other fields are free text.

| Recipient details must be valid                                                                                          |||||
| #Description                           | Field       | Value            | Is Valid? | Error Message?                         |
| -------------------------------------- | ----------- | ---------------- | --------- | -------------------------------------- |
| Name is not provided                   | Name        |                  | false     | The recipient name must be provided    |
| Name is provided as white space        | Name        | " "              | false     | The recipient name must be provided    |
| Valid name is provided                 | Name        | name_123         | true      |                                        |
| Address is not provided                | Address     |                  | false     | The recipient address must be provided |
| Address is provided as white space     | Address     | " "              | false     | The recipient address must be provided |
| Address is provided                    | Address     | address_123      | true      |                                        |
| Email is not provided                  | Email       |                  | false     | Email address must be provided         |
| Email is provided as white space       | Email       | " "              | false     | Email address must be provided         |
| Not valid email is provided            | Email       | email@domain     | false     | Email address must be valid            |
| Valid Email is provided                | Email       | email@domain.com | true      |                                        |
| PhoneNumber is not provided            | PhoneNumber |                  | false     | The phone number must be provided      |
| PhoneNumber is provided as white space | PhoneNumber | ""               | false     | The phone number must be provided      |
| PhoneNumber is provided                | PhoneNumber | phonenumber_123  | true      |                                        |


