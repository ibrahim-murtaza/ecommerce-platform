# E-Commerce Platform UI - Testing Checklist

Use this checklist to verify all features are working correctly.

## 🚀 Pre-Testing Setup

- [ ] Database is running and accessible
- [ ] Database has data (products, categories, users)
- [ ] Connection string is correct in `ECommerceContext.cs`
- [ ] Project builds without errors: `dotnet build`
- [ ] Application launches: `dotnet run --project ECommerce.UI`

---

## ✅ 1. Login & Registration Tests

### Registration (New User)

- [ ] Open application - LoginWindow appears
- [ ] Click "Register" tab
- [ ] Try submitting with empty fields → Error message shown
- [ ] Try password < 6 characters → Error message shown
- [ ] Try mismatched passwords → Error message shown
- [ ] Fill all fields correctly and register
- [ ] Success message appears
- [ ] Try registering same email again → Error "Email already registered"

### Login

- [ ] Enter invalid email → Error message
- [ ] Enter valid email but wrong password → Error message
- [ ] Enter correct credentials → Successfully logs in
- [ ] MainWindow opens after successful login
- [ ] LoginWindow closes

### BLL Toggle (Login Screen)

- [ ] Click "Toggle BLL Type" button
- [ ] Mode indicator updates (LINQ ↔ Stored Procedure)
- [ ] Message confirms switch
- [ ] Login still works after toggle

---

## ✅ 2. Main Window (Product Listing) Tests

### Initial Load

- [ ] MainWindow displays after login
- [ ] Welcome message shows user's first name
- [ ] BLL mode indicator shows current mode
- [ ] Products display in grid layout
- [ ] Each product shows: name, category, price, stock
- [ ] "View Details" and "Add to Cart" buttons visible

### Category Filtering

- [ ] Category dropdown populates with categories
- [ ] "All Categories" is default selection
- [ ] Select a category → Products filter correctly
- [ ] Only products from selected category show
- [ ] "Show All Products" button → Shows all products again

### Low Stock Products

- [ ] Click "Low Stock Products" button
- [ ] Only low stock items display
- [ ] Products match those from database view
- [ ] Click "Show All" → All products return

### Header Buttons

- [ ] "View Cart" button shows count (initially 0)
- [ ] "Admin" button is visible
- [ ] "Toggle BLL" button is visible
- [ ] "Logout" button is visible
- [ ] BLL mode indicator is visible

### BLL Toggle (Main Window)

- [ ] Click "Toggle BLL" button
- [ ] Mode indicator updates
- [ ] Products reload
- [ ] Cart count updates
- [ ] All data still displays correctly

### Quick Add to Cart

- [ ] Click "Add to Cart" on a product
- [ ] Success message appears
- [ ] Cart count in header increases
- [ ] Click multiple times on different products
- [ ] Cart count reflects total items

### Refresh

- [ ] Click "Refresh" button
- [ ] Products reload
- [ ] Cart count updates

---

## ✅ 3. Product Detail Window Tests

### Opening Detail Window

- [ ] Click "View Details" on any product
- [ ] ProductDetailWindow opens as dialog
- [ ] Product name displays correctly
- [ ] Category displays correctly
- [ ] Price displays correctly
- [ ] Stock quantity displays correctly
- [ ] Product ID displays correctly
- [ ] Quantity defaults to 1

### Quantity Controls

- [ ] Click "-" button → Quantity decreases (min 1)
- [ ] Click "+" button → Quantity increases
- [ ] Try to exceed stock → Error message
- [ ] Manual entry in quantity box works
- [ ] Invalid quantity (letters) → Error on add

### Add to Cart

- [ ] Select quantity and click "Add to Cart"
- [ ] Success message appears
- [ ] Can add multiple times
- [ ] Stock validation works
- [ ] Close window → Returns to main window
- [ ] Cart count in main window updated

---

## ✅ 4. Shopping Cart Window Tests

### Opening Cart

- [ ] Click "View Cart" button from main window
- [ ] CartWindow opens as dialog
- [ ] All cart items display
- [ ] Each item shows: name, price, quantity, subtotal
- [ ] Total amount displays at bottom

### Empty Cart

- [ ] With empty cart → "Your cart is empty" message
- [ ] Checkout button is disabled
- [ ] Clear Cart button is disabled

### Cart with Items

- [ ] Items display correctly
- [ ] Product names visible
- [ ] Prices shown correctly
- [ ] Quantities accurate
- [ ] Subtotals calculated correctly
- [ ] Total sum is accurate

### Quantity Updates

- [ ] Click "-" button → Quantity decreases (min 1)
- [ ] Click "+" button → Quantity increases
- [ ] Cannot exceed product stock
- [ ] Subtotal updates immediately
- [ ] Total updates immediately

### Remove Items

- [ ] Click delete button on an item
- [ ] Item removed from cart
- [ ] Total recalculates
- [ ] Display updates immediately

### Clear Cart

- [ ] Click "Clear Cart" button
- [ ] Confirmation dialog appears
- [ ] Click "No" → Cart unchanged
- [ ] Click "Yes" → All items removed
- [ ] "Your cart is empty" message appears

### Navigation

- [ ] "Continue Shopping" → Returns to main window
- [ ] "Proceed to Checkout" → Opens CheckoutWindow
- [ ] Close button → Returns to main window

---

## ✅ 5. Checkout Window Tests

### Opening Checkout

- [ ] From cart with items, click "Proceed to Checkout"
- [ ] CheckoutWindow opens
- [ ] User's address pre-filled
- [ ] User's city pre-filled
- [ ] Postal code field empty
- [ ] Cart total displays
- [ ] Order total displays (same as cart total)

### Validation

- [ ] Try submitting with empty address → Error
- [ ] Try submitting with empty city → Error
- [ ] Try submitting with empty postal code → Error
- [ ] All fields required

### Place Order

- [ ] Fill all shipping info
- [ ] Click "Place Order"
- [ ] Success message appears
- [ ] "Place Order" button disabled
- [ ] Window auto-closes after 2 seconds
- [ ] Return to main window
- [ ] Cart count now shows 0
- [ ] Cart is empty

### Cancel

- [ ] Click "Cancel" button
- [ ] Window closes
- [ ] Cart still has items
- [ ] Return to cart or main window

---

## ✅ 6. Admin Dashboard Tests

### Opening Admin Dashboard

- [ ] From main window, click "Admin" button
- [ ] AdminDashboardWindow opens
- [ ] Four tabs visible: Low Stock, Orders, Revenue, Statistics

### Low Stock Products Tab

- [ ] Low Stock tab is active by default
- [ ] DataGrid displays low stock products
- [ ] Columns: Product ID, Name, Category, Price, Stock
- [ ] Data matches database view
- [ ] Click "Refresh" → Data reloads

### Orders Tab

- [ ] Click "Orders" tab
- [ ] All orders display in DataGrid
- [ ] Columns: Order ID, Date, User ID, Total, Status, Address
- [ ] Status dropdown populates (All, Pending, etc.)
- [ ] Select status → Orders filter correctly
- [ ] "Show All Orders" → All orders display
- [ ] "Refresh" → Data reloads

### Revenue Tab

- [ ] Click "Revenue" tab
- [ ] Start date picker visible
- [ ] End date picker visible
- [ ] Defaults: Start = 1 month ago, End = today
- [ ] Select date range
- [ ] Click "Calculate Revenue"
- [ ] Revenue amount displays
- [ ] Date range displayed below amount
- [ ] Try invalid range (start > end) → Error message

### Statistics Tab

- [ ] Click "Statistics" tab
- [ ] Three stat boxes display
- [ ] "Total Products" shows count
- [ ] "Total Orders" shows count
- [ ] "Total Users" shows count
- [ ] Click "Refresh Statistics" → Numbers update

---

## ✅ 7. Logout Test

- [ ] From main window, click "Logout" button
- [ ] MainWindow closes
- [ ] LoginWindow opens
- [ ] Session cleared (SessionManager.CurrentUser = null)
- [ ] Cannot access protected features without login

---

## ✅ 8. BLL Type Switching (End-to-End)

### Switch to LINQ Mode

- [ ] Login with LINQ mode active
- [ ] Browse products → Data loads
- [ ] Add to cart → Works
- [ ] View cart → Data loads
- [ ] Checkout → Order created
- [ ] Admin dashboard → All tabs work

### Switch to SP Mode

- [ ] Toggle to Stored Procedure mode
- [ ] Browse products → Data loads
- [ ] Add to cart → Works
- [ ] View cart → Data loads
- [ ] Checkout → Order created
- [ ] Admin dashboard → All tabs work

### Mode Persistence

- [ ] Toggle mode in login screen
- [ ] Login → Mode persists in main window
- [ ] Toggle in main window → All features use new mode
- [ ] No errors when switching modes

---

## ✅ 9. UI/UX Tests

### Visual Appearance

- [ ] All windows have proper titles
- [ ] Colors are consistent
- [ ] Buttons have appropriate colors (green for success, red for danger)
- [ ] Text is readable
- [ ] Layouts don't overlap
- [ ] No cut-off text

### Responsiveness

- [ ] Windows open in center of screen
- [ ] Dialog windows appear centered on parent
- [ ] No UI freezing during operations
- [ ] Loading/operations feel responsive

### Error Handling

- [ ] All errors show user-friendly messages
- [ ] Error messages displayed prominently
- [ ] No application crashes
- [ ] Can recover from errors

### Navigation Flow

- [ ] Logical flow from login → shopping → checkout
- [ ] Can navigate back easily
- [ ] Modal dialogs work correctly
- [ ] Cannot interact with parent when dialog open

---

## ✅ 10. Edge Cases & Stress Tests

### Empty States

- [ ] Empty cart displays message correctly
- [ ] No products in category displays appropriately
- [ ] No low stock products handled gracefully
- [ ] No orders in admin panel handled

### Boundary Testing

- [ ] Add product with quantity = stock limit
- [ ] Try adding with quantity > stock
- [ ] Add many items to cart (10+)
- [ ] Create order with many items

### Data Validation

- [ ] Email validation (invalid format)
- [ ] Password validation (too short)
- [ ] Quantity validation (negative, zero, letters)
- [ ] Date validation (invalid ranges)

### Concurrent Operations

- [ ] Toggle BLL mode multiple times quickly
- [ ] Add multiple products rapidly
- [ ] Update cart quantities rapidly
- [ ] Refresh multiple times

---

## 📊 Test Results Summary

| Category             | Passed  | Failed | Notes |
| -------------------- | ------- | ------ | ----- |
| Login & Registration | ** / ** | \_\_   |       |
| Main Window          | ** / ** | \_\_   |       |
| Product Details      | ** / ** | \_\_   |       |
| Shopping Cart        | ** / ** | \_\_   |       |
| Checkout             | ** / ** | \_\_   |       |
| Admin Dashboard      | ** / ** | \_\_   |       |
| Logout               | ** / ** | \_\_   |       |
| BLL Switching        | ** / ** | \_\_   |       |
| UI/UX                | ** / ** | \_\_   |       |
| Edge Cases           | ** / ** | \_\_   |       |

**Overall**: **\_** / **\_** tests passed

---

## 🐛 Bug Report Template

If you find issues, document them:

```
Bug #: ___
Component: _____________
Description:
_____________________________

Steps to Reproduce:
1.
2.
3.

Expected Result:
_____________________________

Actual Result:
_____________________________

BLL Mode: [ ] LINQ  [ ] SP  [ ] Both

Severity: [ ] Critical  [ ] High  [ ] Medium  [ ] Low
```

---

## ✅ Sign-Off

- [ ] All critical tests passed
- [ ] All major features working
- [ ] BLL switching verified in both modes
- [ ] Documentation reviewed
- [ ] Ready for demonstration

**Tested By**: ********\_********
**Date**: ********\_********
**Version**: Phase 3 - December 10, 2025

---

🎉 **Testing Complete!** All features implemented and verified.
