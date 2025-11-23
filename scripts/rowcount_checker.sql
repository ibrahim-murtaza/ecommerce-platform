SELECT 'Category' AS TableName, COUNT(*) AS [RowCount] FROM Category
UNION ALL SELECT 'Product', COUNT(*) FROM Product
UNION ALL SELECT 'User', COUNT(*) FROM [User]
UNION ALL SELECT 'Admin', COUNT(*) FROM Admin
UNION ALL SELECT 'Cart', COUNT(*) FROM Cart
UNION ALL SELECT 'Order', COUNT(*) FROM [Order]
UNION ALL SELECT 'OrderItem', COUNT(*) FROM OrderItem;