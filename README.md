# Order Saga POC

## Overview

This project is a **Proof of Concept (POC)** that demonstrates how to implement a **Saga orchestration pattern** in a distributed system using:

- .NET 8
- MassTransit
- RabbitMQ
- SQL Server
- MongoDB
- Docker

The purpose of this POC is **educational**. It shows how multiple microservices can collaborate to process an order using asynchronous messaging and a saga orchestrator.

The system simulates a simplified **order processing workflow** composed of four services:

- OrderService (Saga Orchestrator)
- InventoryService
- PaymentService
- ShippingService

Each service communicates through **RabbitMQ** using **MassTransit**.

The **OrderService** contains a **Saga State Machine** that orchestrates the entire process.

---

# Architecture

The system uses an **orchestrated Saga pattern**.

OrderService is responsible for coordinating the workflow between services.

The services involved are:

OrderService  
InventoryService  
PaymentService  
ShippingService  

Communication between services happens through **events and commands** over RabbitMQ.

The saga state is persisted in **SQL Server**, while **MongoDB is used to log the saga timeline for observability purposes**.

---

# Order Processing Workflow

The order workflow follows this sequence:

1. Order is submitted
2. Inventory is reserved
3. Payment is processed
4. Shipment is created

The saga controls the flow and handles compensations if something fails.

### Successful flow

OrderSubmitted  
→ ReserveInventory  
→ InventoryReserved  
→ ProcessPayment  
→ PaymentSucceeded  
→ CreateShipment  
→ ShipmentCreated  
→ Saga Completed

### Compensation flows

If inventory fails:

OrderSubmitted  
→ InventoryRejected  
→ Saga Failed

If payment fails:

PaymentFailed  
→ ReleaseInventory  
→ Saga Failed

If shipment fails:

ShipmentFailed  
→ RefundPayment  
→ ReleaseInventory  
→ Saga Failed

---

# Technologies Used

.NET 8  
MassTransit  
RabbitMQ  
SQL Server  
MongoDB  
Docker  
Docker Compose

---

# Services

## OrderService

Responsible for:

- Starting the order process
- Hosting the Saga State Machine
- Orchestrating commands and events
- Persisting saga state in SQL Server
- Logging saga timeline to MongoDB

---

## InventoryService

Responsible for:

- Reserving inventory
- Publishing inventory success or rejection events

---

## PaymentService

Responsible for:

- Processing payments
- Publishing payment success or failure events

---

## ShippingService

Responsible for:

- Creating shipments
- Publishing shipment success or failure events

---

# Saga Persistence

Saga state is stored in **SQL Server** using **Entity Framework Core**.

The saga record includes:

CorrelationId  
OrderId  
InventoryReserved  
PaymentProcessed  
ShippingCreated  
CurrentState  

When the saga reaches the final state it is removed automatically due to:

SetCompletedWhenFinalized()

This is the default behavior of MassTransit saga repositories.

---

# Saga Timeline Logging

For learning and observability purposes, the POC logs every saga transition into **MongoDB**.

Each event generates a document containing:

CorrelationId  
OrderId  
Event  
State  
Timestamp  
Payload  

This creates a **timeline of the order execution**, which can be inspected using Mongo Express.

---

# Running the System

The entire environment is containerized using Docker.

Services started by Docker Compose:

RabbitMQ  
SQL Server  
MongoDB  
Mongo Express  
OrderService  
InventoryService  
PaymentService  
ShippingService  

---

## Docker commands
Start the environment:

```cmd
docker compose up --build
```

To stop the environment:
```cmd
docker compose down
```

---

## System URLs

### Order Service
http://localhost:5001

Health endpoint:
http://localhost:5001/

---

## RabbitMQ Management
http://localhost:15672

Credentials:

username  
guest

password  
guest

You can inspect:

Queues  
Messages  
Consumers  
Error queues  

---

## Mongo Express (MongoDB UI)
http://localhost:8081

Credentials:

username  
admin

password  
admin123

Database used:
OrderSagaLogs

Collection:
logs

This collection stores the saga timeline.

---

## SQL Server Access

The saga state is stored in SQL Server.

Connection parameters:

Server  
localhost,5282

Authentication  
SQL Server Authentication

User  
sa

Password  
2026@Pass.Word

Database  
OrderSagaDb

You can inspect saga state using SSMS with:
```sq
SELECT * FROM OrderSagaStates
```

Note:

When the saga finishes successfully, the record is automatically removed due to the `SetCompletedWhenFinalized()` configuration.

---

## Triggering the Workflow

To start the order process, the project includes a REST client file.

Location:
./http/orders/create-order.rest

This file can be executed using the **REST Client extension in VS Code**.

The request triggers the entire workflow by publishing the `OrderSubmitted` event.

Example request:
``````http
POST http://localhost:5001/orders
```

Response example:
```json
202 Accepted
{
"message": "Order creation started",
"orderId": "GUID"
}
```

---

## Observing the System

After triggering an order you can observe the system through multiple tools.

### RabbitMQ

View message flow between services.

Queues involved:

reserve-inventory  
process-payment  
create-shipment  
release-inventory  
refund-payment  
order-saga-state  

---

### SQL Server

Check saga state:
```sql
SELECT * FROM OrderSagaStates
```

---

### MongoDB

View saga timeline:

Database  
OrderSagaLogs

Collection  
logs

Example document:
```json
{
"correlationId": "...",
"orderId": "...",
"event": "PaymentSucceeded",
"state": "WaitingForShipping",
"timestamp": "...",
"payload": "{...}"
}
```

---

# Educational Goals

This POC demonstrates:

- Saga orchestration pattern
- Distributed communication using RabbitMQ
- MassTransit message-based workflows
- Event-driven microservices
- Compensating transactions
- Saga state persistence
- Observability using MongoDB
- Containerized distributed environments with Docker

---

## Notes

This project is intentionally simplified for educational purposes.

In production systems additional features would normally be included such as:

- Outbox pattern  
- Retries and circuit breakers  
- Observability and tracing  
- Authentication and security  
- Centralized logging  
- Monitoring and alerting  

---

## Conclusion

This POC provides a practical example of implementing **distributed workflows using the Saga pattern** with MassTransit and RabbitMQ.

It illustrates how a central orchestrator can coordinate multiple services while maintaining system consistency through asynchronous messaging and compensating actions.