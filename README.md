# DB Hotel Reservation System 🏨

A modern, responsive desktop hotel reservation and front-desk management system built with **C#** and **Windows Forms (.NET)** in Visual Studio.

---

## 🌟 Key Features

### 1. Landing Screen (`Home`)
- **Visual Branding**: Designed with an aesthetic burgundy and warm cream color scheme (`#7A1F3E`, `#3D2A30`, `#F7E3C8`).
- **Hero Banner**: Features background imagery, custom typography, and clear calls to action.
- **Fluid Navigation**: Instant switching to room listings and reservation flows.

### 2. Room & Suite Showcase (`Form2`)
- **Stay Filter**: Check-In / Check-Out date selection with guest count controls.
- **Room Catalog**: Card grid showcasing available accommodations (Standard, Suite, Deluxe) with specifications (bed count, floor area in m², and pricing per night).
- **Direct Booking**: One-click booking triggers directly linked to the reservation pipeline.
- **Availability Check**: Direct link to the front-desk dashboard.

### 3. Reservation & Billing (`Form3`)
- **Guest Details**: Name and phone number collection with clean input styling.
- **Service Add-ons**: Optional checkboxes for extra amenities (Restaurant Meals, Laundry, Spa, Airport Pickup, Gym) with clear itemized rates.
- **Payment Gateway Options**: Support for local and alternative payment methods including CBE, Telebirr, and other providers.
- **Room Summary Card**: Real-time room summary and rate overview prior to confirmation.

### 4. Front-Desk / Admin Dashboard (`Form4`)
- **Key Metrics Overview**: Real-time summary cards for:
  - Total occupied rooms (e.g., 18 of 40)
  - Today's arrivals
  - Today's departures
  - Pending payments
- **Arrivals Tracker**: Tabular overview showing guest name, assigned room number, and payment status tags (`paid` / `pending`).
- **Inventory Overview**: Breakdown of available rooms categorized by type.
- **Sidebar Menu**: Quick access to all core sections of the app.

### 5. UI & Responsiveness Engine
- **Custom Curvature**: Handled via `GraphicsPath` clipping for rounded cards, picture frames, and action buttons.
- **Adaptive Fullscreen Layout**: Centered content columns and standard right-aligned contact headers on wide and high-resolution monitors.

---

## 📸 Screenshots

| Home Page | Rooms & Suites |
| :---: | :---: |
| ![Home](Screenshots/Home.png) | ![Rooms and Suites](Screenshots/Rooms.png) |

| Reservation Screen | Management Dashboard |
| :---: | :---: |
| ![Reservation](Screenshots/Reservation.png) | ![Dashboard](Screenshots/Dashboard.png) |

> *Tip: Save your screenshot images into a folder named `Screenshots` in your repository matching the names above to display them automatically.*

---

## 📋 Business Rules

### Room Availability

- Only rooms with an `Available` status can be assigned to a new reservation.
- The requested room type is matched before another available room is considered.
- When a reservation is confirmed, the assigned room becomes `Occupied`.
- A checked-out room becomes `Cleaning` until an administrator releases it.
- Cleaning staff or an administrator can return a cleaned room to `Available` status.

### Reservations

- A reservation must include the guest's full name and phone number.
- The check-out date must be later than the check-in date.
- Each reservation is assigned a room and contains its room type, dates, guest details, and total price.
- The total price is calculated from the nightly room rate, number of nights, and selected services.
- A reservation cannot be confirmed when no room is available.

### Additional Services

- Guests may select restaurant meals, laundry, spa, airport pickup, and gym services.
- Each selected service adds its applicable fee to the reservation total.
- Service selections are recalculated whenever they are changed.

### Payments

- Supported payment channels include CBE, Telebirr, and other payment methods.
- A payment reference or receipt code is required before a reservation can be submitted.
- New reservations have a `Pending Verification` payment status.
- Administrators can verify or reject reservation payments from the dashboard.

### Dashboard Management

- Administrators can view reservation arrivals, room availability, and pending payments.
- Administrators can search the reservations table by guest, room, or transaction reference.
- An active guest can be checked out only after selecting the reservation.
- Checking out a guest changes the reservation status to `Departed` and the room status to `Cleaning`.
- Reservation data is stored in memory and is reset when the application closes.

---

## 🛠️ Technology Stack

- **Language:** C#
- **Framework:** .NET Framework / Windows Forms (WinForms)
- **UI Engine:** GDI+ (`System.Drawing`, `System.Drawing.Drawing2D`)
- **IDE:** Visual Studio 2022 / 2026

---

## 📂 Project Architecture

```text
HotelReservation/
├── Home.cs / form1.cs             # Main landing form logic
├── formdesigner.cs                # Designer layout for Home
├── Form2.cs                       # Rooms & suites catalog logic
├── form2designer.cs               # Designer layout for Form2
├── Form3.cs                       # Reservation & guest intake logic
├── form3designer.cs               # Designer layout for Form3
├── Form4.cs                       # Admin dashboard logic
├── form4designer.cs               # Designer layout for Form4
├── Program.cs                     # Application bootstrapper
└── Properties/                    # Assembly info and application resources
