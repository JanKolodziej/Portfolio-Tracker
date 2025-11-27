![Architecture](https://img.shields.io/badge/Architecture-MVVM-blueviolet)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-512bd4)

# 📊 Portfolio Tracker

**Portfolio Tracker** is a Windows desktop application built with **.NET MAUI** that helps users track and analyze their investment performance on XTB accounts.

The app allows users to **import data directly from XTB-generated files**, view profit and loss across multiple accounts, and compare their performance against the **S&P 500 index**.

---

## 🚀 Features

- 📂 **Data Import:** Process trading data seamlessly from XTB export files (CSV/Excel).
- 💰 **Multi-Account Support:** Track earnings across various investment accounts.
- 📈 **Performance Analysis:** View total and account-level profit/loss summaries.
- 📊 **Benchmarking:** Compare your portfolio results directly against S&P 500 performance.
- 🧮 **Aggregation:** Automatic calculation of overall profit summaries.
- 🪄 **Modern UI:** Clean and responsive interface built with .NET MAUI and XAML.

---

## 🧠 Architecture

The project follows the **MVVM (Model-View-ViewModel)** architectural pattern to ensure separation of concerns and testability.

```text
Solution
├── 📂 CoreLibrary (Biblioteka Klas)   # Domain Logic & Data Models
│   ├── Konto.cs                       // Investment account entity
│   ├── KontoSumaryczne.cs             // Aggregated account summary
│   ├── OtwartaPozycja.cs              // Open position model
│   ├── ZamknietaPozycja.cs            // Closed position model
│   ├── SP500Pozycja.cs                // S&P 500 index data
│   ├── SQLiteDane.cs                  // Database connection logic
│   └── OperacjeGotowkowe.cs           // Cash operations logic
│
├── 📂 MauiApp1                        # UI & Application Layer
│   ├── 📁 Data                        // Local database context
│   ├── 📁 Views                       // XAML Pages (User Interface)
│   ├── 📁 ViewModels                  // Presentation Logic (MVVM)
│   └── 📁 Narzedzia (Tools)           // Helpers for charts and file parsing
│
└── 📂 MauiApp1.Test                   # Unit Testing
    ├── UnitTest1.cs
    └── TestyPrzekrojowe.cs
```

---

## 🛠️ Technologies Used

- **C# 12 / .NET 8**
- **.NET MAUI** (Targeting Windows Desktop)
- **XAML** for UI definition
- **SQLite** for local data storage
- **CommunityToolkit.Mvvm** (implied) for MVVM pattern support
- **Metalama Framework** for boilerplate reduction and observability

---

## ⚙️ How to Run

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/JanKolodziej/Portfolio-Tracker.git](https://github.com/JanKolodziej/Portfolio-Tracker.git)
   ```
2. **Open the solution:**
   Open the `.sln` file in **Visual Studio 2022**.
3. **Restore packages:**
   Visual Studio should automatically restore NuGet packages.
4. **Run:**
   Select **Windows Machine** as the target and press `F5`.

---

## 🧩 Future Improvements

- 🌐 Integrate real-time market data API (e.g., Alpha Vantage or Yahoo Finance).
- 🔍 Add advanced filtering and sorting for transaction history.
- 🔐 Implement secure user login and profile management.
- 📱 Expand support to **Android** and **iOS** platforms.

---

## 📚 Libraries & Credits

This project leverages the following libraries:

* [**LiveCharts2**](https://github.com/beto-rodriguez/LiveCharts2) (MIT) – For creating interactive financial charts.
* [**ClosedXML**](https://github.com/ClosedXML/ClosedXML) (MIT) – For parsing Excel files exported from XTB.
* [**Metalama**](https://www.metalama.net/) – Used for Aspect-Oriented Programming (AOP) to handle `INotifyPropertyChanged` and other cross-cutting concerns.

---

## 👤 Author

**Jan Kołodziej** 💼 .NET C# Developer  
📧 [jankolodziej@outlook.com](mailto:jankolodziej@outlook.com)  
🔗 [LinkedIn Profile](https://www.linkedin.com/in/jan-kolodziej-krk/)

---

> **Disclaimer:** *Portfolio Tracker* is an independent project and is not affiliated with, endorsed by, or connected to XTB S.A. It was built for educational purposes and personal portfolio tracking.
