# 🧪 Visual Testing Guide - Course Search Features

## 🎯 **Quick Feature Verification**

### **Access the Application**

- **URL**: http://localhost:5177/
- **Navigation**: Top menu "Course Search" or Footer "Course Search"
- **Direct Route**: http://localhost:5177/course-search

---

## ✅ **Test Scenarios for Each Acceptance Criteria**

### **AC1: Advanced Course Search with Multiple Filters**

#### **Test 1: Multi-Filter Search**

1. **Navigate**: `/course-search`
2. **Set Filters**:
   - Department: "Computer Science"
   - Level: "300 - Advanced"
   - Instructor: "Johnson"
   - Days: Check "Monday" and "Wednesday"
3. **Expected**: Search results update in real-time
4. **Verify**: Only CS 300-level courses by Johnson on Mon/Wed appear

#### **Test 2: Search Debouncing**

1. **Type slowly** in "Search Keywords" field: "Database"
2. **Expected**: No search until you stop typing for 300ms
3. **Verify**: Search indicator shows "Searching..." briefly

#### **Test 3: View Modes**

1. **Click**: "Grid View" button (top-right)
2. **Expected**: Cards layout with course info
3. **Click**: "List View" button
4. **Expected**: Detailed list layout returns

#### **Test 4: Clear Filters**

1. **Set multiple filters** (any combination)
2. **Click**: "Clear Filters" button
3. **Expected**: All filters reset, full course list appears

---

### **AC2: Detailed Course Information with Prerequisites**

#### **Test 5: Course Details Modal**

1. **Click**: "Details" button on any course
2. **Expected**: Modal opens with comprehensive course info
3. **Verify Elements**:
   - Course title and code
   - Instructor and department
   - Credits and difficulty rating
   - Complete schedule with duration
   - Prerequisites (if any) as badges
   - Enrollment status and capacity

#### **Test 6: Prerequisites Validation**

1. **Find course with prerequisites** (e.g., "Advanced Database Systems")
2. **Click**: "Details" button
3. **Expected**: Prerequisites shown as blue badges
4. **Click**: "Enroll Now" (if available)
5. **Expected**: Validation error about missing prerequisites

---

### **AC3: Course Comparison Side-by-Side Functionality**

#### **Test 7: Multi-Course Selection**

1. **Check "Compare" boxes** on 2-3 different courses
2. **Expected**: "Compare (X)" button appears in top-right
3. **Click**: "Compare (X)" button
4. **Expected**: Comparison modal opens with selected courses

#### **Test 8: Comparison Table**

1. **In comparison modal**, scroll through table
2. **Verify Rows**:
   - Department, Credits, Instructor
   - Schedule and Location
   - Prerequisites
   - Enrollment status
   - Difficulty and Workload (if available)
   - Course descriptions

#### **Test 9: Smart Recommendations**

1. **Scroll to bottom** of comparison modal
2. **Expected**: Summary cards showing:
   - "Lowest Difficulty" course
   - "Least Workload" course
   - "Most Available" course
3. **Click**: "Enroll in Recommended" button
4. **Expected**: Enrollment attempt for recommended course

---

### **AC4: One-Click Enrollment with Real-Time Validation**

#### **Test 10: Successful Enrollment**

1. **Find available course** without prerequisites
2. **Click**: "Enroll Now" button
3. **Expected**:
   - Success toast notification
   - Button changes to "Enrolled" (green, disabled)
   - Course status updates immediately

#### **Test 11: Prerequisites Validation**

1. **Find course with prerequisites** you don't meet
2. **Click**: "Enroll Now" button
3. **Expected**:
   - Red error toast: "Cannot enroll: Missing prerequisites: [codes]"
   - Enrollment button remains unchanged

#### **Test 12: Schedule Conflict Detection**

1. **Enroll in course** with specific time
2. **Try enrolling** in another course at same time
3. **Expected**: Warning about schedule conflict

---

### **AC5: Waitlist Status and Automatic Notification System**

#### **Test 13: Join Waitlist**

1. **Find "full" course** (enrollment status shows "Waitlist")
2. **Click**: "Join Waitlist" button (yellow)
3. **Expected**:
   - Blue info toast: "Added to waitlist for [course code]"
   - Button changes to show waitlist status

#### **Test 14: Waitlist Notifications**

1. **After joining waitlist**, check course details
2. **Expected**: Shows waitlist position and notification preferences
3. **Verify**: Course status indicates "On Waitlist"

#### **Test 15: Auto-Enrollment from Waitlist**

1. **Mock scenario**: Use browser dev tools or API directly
2. **Simulate**: Spot becomes available in waitlisted course
3. **Expected**:
   - Green success toast: "Automatically enrolled from waitlist!"
   - Status changes to "Enrolled"

---

## 🧪 **Integration Tests**

### **Test 16: Error Handling**

1. **Disconnect internet** temporarily
2. **Try any search/enrollment action**
3. **Expected**: User-friendly error messages, not crashes
4. **Reconnect**: Normal functionality resumes

### **Test 17: Empty Results**

1. **Set filters** that return no results (e.g., "999 - Graduate" level)
2. **Expected**:
   - "No courses found" message with search icon
   - Suggestion to adjust filters
   - Clear way to reset search

### **Test 18: Pagination**

1. **Clear all filters** to show all courses
2. **If more than 12 courses**: Pagination controls appear at bottom
3. **Click**: Next/Previous page numbers
4. **Expected**: Results update, page numbers highlight correctly

### **Test 19: Mobile Responsiveness**

1. **Resize browser** to mobile width (< 768px)
2. **Expected**:
   - Filters stack vertically
   - Grid view shows single column
   - Touch-friendly button sizes
   - Navigation collapses appropriately

---

## 🎨 **Visual Elements to Verify**

### **Color Coding System**:

- 🟢 **Green**: Enrolled status, success messages
- 🔵 **Blue**: Available courses, info messages
- 🟡 **Yellow**: Waitlist status, warning messages
- 🔴 **Red**: Full courses, error messages
- ⚫ **Gray**: Disabled states, secondary info

### **Interactive Elements**:

- **Hover Effects**: Buttons and cards lift slightly
- **Loading States**: Spinners during search/enrollment
- **Toast Notifications**: Slide in from top-right corner
- **Modal Animations**: Smooth fade in/out
- **Responsive Breakpoints**: Clean layout on all screen sizes

### **Accessibility Features**:

- **Keyboard Navigation**: Tab through all interactive elements
- **Screen Reader**: Proper ARIA labels and descriptions
- **Focus Indicators**: Clear visual focus states
- **Color Contrast**: All text meets WCAG standards

---

## 🚀 **Performance Verification**

### **Search Performance**:

- **Debouncing**: 300ms delay on text input
- **Real-time Updates**: Dropdown/checkbox changes are instant
- **Network Efficiency**: No duplicate API calls

### **UI Responsiveness**:

- **Smooth Animations**: No janky transitions
- **Fast Rendering**: Course cards appear quickly
- **Memory Usage**: No memory leaks during extended use

---

## 📊 **Success Metrics**

When demonstrating, you should see:

- ✅ **Zero Console Errors** in browser dev tools
- ✅ **100% Test Pass Rate** (19/19 tests)
- ✅ **Clean Build** with no TypeScript errors
- ✅ **Responsive Design** on all screen sizes
- ✅ **Intuitive User Flow** from search to enrollment
- ✅ **Real-time Feedback** for all user actions

This testing guide ensures you can confidently demonstrate every aspect of the Course Search and Enrollment Interface!
