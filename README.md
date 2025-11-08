# E-Commerce Database - Group 22

## Team Members
- Ibrahim Murtaza - 26100050
- Jibran Mazhar - 26100225
- Muhammad Ahsan Khan Lodhi - 26100096
- Muhammad Arsalan Amjad - 26100003

## Setup Instructions

### Run Schema Scripts
Execute in VS Code in order:
1. `database/schema/01_create_database.sql`
2. `database/schema/02_create_tables.sql`

### Verify
```sql
USE ECommerceDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;
```

## Database Schema

### Tables
- **User**: Customer accounts
- **Category**: Product categories  
- **Product**: Product catalog with inventory
- **Cart**: Shopping cart items
- **Order**: Customer orders
- **OrderItem**: Order line items
- **Admin**: Administrative users

### Relationships
- User (1) → (N) Cart, Order
- Category (1) → (N) Product
- Product (1) → (N) Cart, OrderItem
- Order (1) → (N) OrderItem