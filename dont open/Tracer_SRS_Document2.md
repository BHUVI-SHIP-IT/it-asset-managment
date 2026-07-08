# Enterprise IT Asset Management System (Project Tracer)
## Document 2: Software Requirements Specification (SRS)

**Prepared By:** Sakthivel P, Principal Enterprise Architect
**Document Version:** 1.0
**Status:** Approved for Engineering Implementation

---

## 1. Introduction
This Software Requirements Specification (SRS) exhaustively details the functional and business requirements for Project Tracer. It encompasses all modules, business rules, actor interactions, and edge cases necessary to construct an enterprise-grade ITAM platform utilizing Clean Architecture, ASP.NET Core 9, MediatR, and Angular 20+.

## 2. Module Specifications

### 2.1 Module: Dashboard

#### 2.1.1 Purpose
The primary purpose of the Dashboard module is to establish strict governance and lifecycle management capabilities. Provides a real-time, consolidated view of enterprise asset metrics, alerts, and system health.

#### 2.1.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Dashboard state changes instantly without page reloads.

#### 2.1.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.1.4 Business Rules
1. **Immutability of History:** Any mutation to a Dashboard record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Dashboard states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Dashboard data must strictly validate against the executing user's Company ID.

#### 2.1.5 Functional Requirements
* **FR-1.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Dashboard records.
* **FR-1.2:** The Angular UI SHALL display a paginated, filterable grid of Dashboard utilizing server-side processing.
* **FR-1.3:** The system SHALL support bulk importing of Dashboard via CSV with pre-flight validation.
* **FR-1.4:** The system SHALL support relational mapping of Dashboard to other enterprise entities (e.g., Users, Locations).

#### 2.1.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.1.7 Error Conditions
* **404 Not Found:** Thrown when a queried Dashboard ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Dashboard creation violates unique constraints (e.g., duplicate names/tags).

#### 2.1.8 Permissions
* `DASHBOARD.VIEW`
* `DASHBOARD.CREATE`
* `DASHBOARD.EDIT`
* `DASHBOARD.DELETE`
* `DASHBOARD.CHECKOUT` (if applicable)

#### 2.1.9 Notifications
* Event `OnDashboardCreated`: Dispatches an informational webhook payload.
* Event `OnDashboardAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.1.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Dashboard records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Dashboard utilization, depreciation, and assignment history over a given temporal window.

#### 2.1.11 Audit Entries
* Action: `CREATE`, Target: `Dashboard`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Dashboard`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Dashboard`, Payload: Deletion timestamp and initiating actor.

#### 2.1.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Dashboard.
* Enhanced mobile offline capabilities for modifying Dashboard in disconnected audit environments.

---

### 2.2 Module: Assets

#### 2.2.1 Purpose
The primary purpose of the Assets module is to establish strict governance and lifecycle management capabilities. The core entity representing physical hardware managed within the enterprise lifecycle.

#### 2.2.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Assets state changes instantly without page reloads.

#### 2.2.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.2.4 Business Rules
1. **Immutability of History:** Any mutation to a Assets record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Assets states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Assets data must strictly validate against the executing user's Company ID.

#### 2.2.5 Functional Requirements
* **FR-2.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Assets records.
* **FR-2.2:** The Angular UI SHALL display a paginated, filterable grid of Assets utilizing server-side processing.
* **FR-2.3:** The system SHALL support bulk importing of Assets via CSV with pre-flight validation.
* **FR-2.4:** The system SHALL support relational mapping of Assets to other enterprise entities (e.g., Users, Locations).

#### 2.2.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.2.7 Error Conditions
* **404 Not Found:** Thrown when a queried Assets ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Assets creation violates unique constraints (e.g., duplicate names/tags).

#### 2.2.8 Permissions
* `ASSETS.VIEW`
* `ASSETS.CREATE`
* `ASSETS.EDIT`
* `ASSETS.DELETE`
* `ASSETS.CHECKOUT` (if applicable)

#### 2.2.9 Notifications
* Event `OnAssetsCreated`: Dispatches an informational webhook payload.
* Event `OnAssetsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.2.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Assets records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Assets utilization, depreciation, and assignment history over a given temporal window.

#### 2.2.11 Audit Entries
* Action: `CREATE`, Target: `Assets`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Assets`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Assets`, Payload: Deletion timestamp and initiating actor.

#### 2.2.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Assets.
* Enhanced mobile offline capabilities for modifying Assets in disconnected audit environments.

---

### 2.3 Module: Asset Models

#### 2.3.1 Purpose
The primary purpose of the Asset Models module is to establish strict governance and lifecycle management capabilities. Templates defining standard configurations, depreciation rules, and attributes for grouped assets.

#### 2.3.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Asset Models state changes instantly without page reloads.

#### 2.3.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.3.4 Business Rules
1. **Immutability of History:** Any mutation to a Asset Models record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Asset Models states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Asset Models data must strictly validate against the executing user's Company ID.

#### 2.3.5 Functional Requirements
* **FR-3.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Asset Models records.
* **FR-3.2:** The Angular UI SHALL display a paginated, filterable grid of Asset Models utilizing server-side processing.
* **FR-3.3:** The system SHALL support bulk importing of Asset Models via CSV with pre-flight validation.
* **FR-3.4:** The system SHALL support relational mapping of Asset Models to other enterprise entities (e.g., Users, Locations).

#### 2.3.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.3.7 Error Conditions
* **404 Not Found:** Thrown when a queried Asset Models ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Asset Models creation violates unique constraints (e.g., duplicate names/tags).

#### 2.3.8 Permissions
* `ASSET_MODELS.VIEW`
* `ASSET_MODELS.CREATE`
* `ASSET_MODELS.EDIT`
* `ASSET_MODELS.DELETE`
* `ASSET_MODELS.CHECKOUT` (if applicable)

#### 2.3.9 Notifications
* Event `OnAssetModelsCreated`: Dispatches an informational webhook payload.
* Event `OnAssetModelsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.3.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Asset Models records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Asset Models utilization, depreciation, and assignment history over a given temporal window.

#### 2.3.11 Audit Entries
* Action: `CREATE`, Target: `Asset Models`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Asset Models`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Asset Models`, Payload: Deletion timestamp and initiating actor.

#### 2.3.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Asset Models.
* Enhanced mobile offline capabilities for modifying Asset Models in disconnected audit environments.

---

### 2.4 Module: Manufacturers

#### 2.4.1 Purpose
The primary purpose of the Manufacturers module is to establish strict governance and lifecycle management capabilities. Entities that produce the hardware and software managed within the system.

#### 2.4.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Manufacturers state changes instantly without page reloads.

#### 2.4.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.4.4 Business Rules
1. **Immutability of History:** Any mutation to a Manufacturers record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Manufacturers states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Manufacturers data must strictly validate against the executing user's Company ID.

#### 2.4.5 Functional Requirements
* **FR-4.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Manufacturers records.
* **FR-4.2:** The Angular UI SHALL display a paginated, filterable grid of Manufacturers utilizing server-side processing.
* **FR-4.3:** The system SHALL support bulk importing of Manufacturers via CSV with pre-flight validation.
* **FR-4.4:** The system SHALL support relational mapping of Manufacturers to other enterprise entities (e.g., Users, Locations).

#### 2.4.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.4.7 Error Conditions
* **404 Not Found:** Thrown when a queried Manufacturers ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Manufacturers creation violates unique constraints (e.g., duplicate names/tags).

#### 2.4.8 Permissions
* `MANUFACTURERS.VIEW`
* `MANUFACTURERS.CREATE`
* `MANUFACTURERS.EDIT`
* `MANUFACTURERS.DELETE`
* `MANUFACTURERS.CHECKOUT` (if applicable)

#### 2.4.9 Notifications
* Event `OnManufacturersCreated`: Dispatches an informational webhook payload.
* Event `OnManufacturersAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.4.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Manufacturers records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Manufacturers utilization, depreciation, and assignment history over a given temporal window.

#### 2.4.11 Audit Entries
* Action: `CREATE`, Target: `Manufacturers`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Manufacturers`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Manufacturers`, Payload: Deletion timestamp and initiating actor.

#### 2.4.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Manufacturers.
* Enhanced mobile offline capabilities for modifying Manufacturers in disconnected audit environments.

---

### 2.5 Module: Categories

#### 2.5.1 Purpose
The primary purpose of the Categories module is to establish strict governance and lifecycle management capabilities. Hierarchical classification structures for organizing assets, components, and consumables.

#### 2.5.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Categories state changes instantly without page reloads.

#### 2.5.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.5.4 Business Rules
1. **Immutability of History:** Any mutation to a Categories record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Categories states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Categories data must strictly validate against the executing user's Company ID.

#### 2.5.5 Functional Requirements
* **FR-5.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Categories records.
* **FR-5.2:** The Angular UI SHALL display a paginated, filterable grid of Categories utilizing server-side processing.
* **FR-5.3:** The system SHALL support bulk importing of Categories via CSV with pre-flight validation.
* **FR-5.4:** The system SHALL support relational mapping of Categories to other enterprise entities (e.g., Users, Locations).

#### 2.5.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.5.7 Error Conditions
* **404 Not Found:** Thrown when a queried Categories ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Categories creation violates unique constraints (e.g., duplicate names/tags).

#### 2.5.8 Permissions
* `CATEGORIES.VIEW`
* `CATEGORIES.CREATE`
* `CATEGORIES.EDIT`
* `CATEGORIES.DELETE`
* `CATEGORIES.CHECKOUT` (if applicable)

#### 2.5.9 Notifications
* Event `OnCategoriesCreated`: Dispatches an informational webhook payload.
* Event `OnCategoriesAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.5.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Categories records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Categories utilization, depreciation, and assignment history over a given temporal window.

#### 2.5.11 Audit Entries
* Action: `CREATE`, Target: `Categories`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Categories`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Categories`, Payload: Deletion timestamp and initiating actor.

#### 2.5.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Categories.
* Enhanced mobile offline capabilities for modifying Categories in disconnected audit environments.

---

### 2.6 Module: Suppliers

#### 2.6.1 Purpose
The primary purpose of the Suppliers module is to establish strict governance and lifecycle management capabilities. Vendors and external partners from whom assets, licenses, and consumables are procured.

#### 2.6.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Suppliers state changes instantly without page reloads.

#### 2.6.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.6.4 Business Rules
1. **Immutability of History:** Any mutation to a Suppliers record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Suppliers states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Suppliers data must strictly validate against the executing user's Company ID.

#### 2.6.5 Functional Requirements
* **FR-6.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Suppliers records.
* **FR-6.2:** The Angular UI SHALL display a paginated, filterable grid of Suppliers utilizing server-side processing.
* **FR-6.3:** The system SHALL support bulk importing of Suppliers via CSV with pre-flight validation.
* **FR-6.4:** The system SHALL support relational mapping of Suppliers to other enterprise entities (e.g., Users, Locations).

#### 2.6.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.6.7 Error Conditions
* **404 Not Found:** Thrown when a queried Suppliers ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Suppliers creation violates unique constraints (e.g., duplicate names/tags).

#### 2.6.8 Permissions
* `SUPPLIERS.VIEW`
* `SUPPLIERS.CREATE`
* `SUPPLIERS.EDIT`
* `SUPPLIERS.DELETE`
* `SUPPLIERS.CHECKOUT` (if applicable)

#### 2.6.9 Notifications
* Event `OnSuppliersCreated`: Dispatches an informational webhook payload.
* Event `OnSuppliersAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.6.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Suppliers records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Suppliers utilization, depreciation, and assignment history over a given temporal window.

#### 2.6.11 Audit Entries
* Action: `CREATE`, Target: `Suppliers`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Suppliers`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Suppliers`, Payload: Deletion timestamp and initiating actor.

#### 2.6.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Suppliers.
* Enhanced mobile offline capabilities for modifying Suppliers in disconnected audit environments.

---

### 2.7 Module: Companies

#### 2.7.1 Purpose
The primary purpose of the Companies module is to establish strict governance and lifecycle management capabilities. Tenant boundaries for multi-tenant deployments, ensuring data isolation across distinct business entities.

#### 2.7.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Companies state changes instantly without page reloads.

#### 2.7.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.7.4 Business Rules
1. **Immutability of History:** Any mutation to a Companies record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Companies states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Companies data must strictly validate against the executing user's Company ID.

#### 2.7.5 Functional Requirements
* **FR-7.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Companies records.
* **FR-7.2:** The Angular UI SHALL display a paginated, filterable grid of Companies utilizing server-side processing.
* **FR-7.3:** The system SHALL support bulk importing of Companies via CSV with pre-flight validation.
* **FR-7.4:** The system SHALL support relational mapping of Companies to other enterprise entities (e.g., Users, Locations).

#### 2.7.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.7.7 Error Conditions
* **404 Not Found:** Thrown when a queried Companies ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Companies creation violates unique constraints (e.g., duplicate names/tags).

#### 2.7.8 Permissions
* `COMPANIES.VIEW`
* `COMPANIES.CREATE`
* `COMPANIES.EDIT`
* `COMPANIES.DELETE`
* `COMPANIES.CHECKOUT` (if applicable)

#### 2.7.9 Notifications
* Event `OnCompaniesCreated`: Dispatches an informational webhook payload.
* Event `OnCompaniesAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.7.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Companies records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Companies utilization, depreciation, and assignment history over a given temporal window.

#### 2.7.11 Audit Entries
* Action: `CREATE`, Target: `Companies`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Companies`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Companies`, Payload: Deletion timestamp and initiating actor.

#### 2.7.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Companies.
* Enhanced mobile offline capabilities for modifying Companies in disconnected audit environments.

---

### 2.8 Module: Departments

#### 2.8.1 Purpose
The primary purpose of the Departments module is to establish strict governance and lifecycle management capabilities. Internal organizational units used for logical grouping of users and cost-center allocation.

#### 2.8.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Departments state changes instantly without page reloads.

#### 2.8.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.8.4 Business Rules
1. **Immutability of History:** Any mutation to a Departments record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Departments states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Departments data must strictly validate against the executing user's Company ID.

#### 2.8.5 Functional Requirements
* **FR-8.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Departments records.
* **FR-8.2:** The Angular UI SHALL display a paginated, filterable grid of Departments utilizing server-side processing.
* **FR-8.3:** The system SHALL support bulk importing of Departments via CSV with pre-flight validation.
* **FR-8.4:** The system SHALL support relational mapping of Departments to other enterprise entities (e.g., Users, Locations).

#### 2.8.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.8.7 Error Conditions
* **404 Not Found:** Thrown when a queried Departments ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Departments creation violates unique constraints (e.g., duplicate names/tags).

#### 2.8.8 Permissions
* `DEPARTMENTS.VIEW`
* `DEPARTMENTS.CREATE`
* `DEPARTMENTS.EDIT`
* `DEPARTMENTS.DELETE`
* `DEPARTMENTS.CHECKOUT` (if applicable)

#### 2.8.9 Notifications
* Event `OnDepartmentsCreated`: Dispatches an informational webhook payload.
* Event `OnDepartmentsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.8.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Departments records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Departments utilization, depreciation, and assignment history over a given temporal window.

#### 2.8.11 Audit Entries
* Action: `CREATE`, Target: `Departments`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Departments`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Departments`, Payload: Deletion timestamp and initiating actor.

#### 2.8.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Departments.
* Enhanced mobile offline capabilities for modifying Departments in disconnected audit environments.

---

### 2.9 Module: Locations

#### 2.9.1 Purpose
The primary purpose of the Locations module is to establish strict governance and lifecycle management capabilities. Geographical or physical sites where assets are deployed, stored, or audited.

#### 2.9.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Locations state changes instantly without page reloads.

#### 2.9.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.9.4 Business Rules
1. **Immutability of History:** Any mutation to a Locations record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Locations states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Locations data must strictly validate against the executing user's Company ID.

#### 2.9.5 Functional Requirements
* **FR-9.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Locations records.
* **FR-9.2:** The Angular UI SHALL display a paginated, filterable grid of Locations utilizing server-side processing.
* **FR-9.3:** The system SHALL support bulk importing of Locations via CSV with pre-flight validation.
* **FR-9.4:** The system SHALL support relational mapping of Locations to other enterprise entities (e.g., Users, Locations).

#### 2.9.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.9.7 Error Conditions
* **404 Not Found:** Thrown when a queried Locations ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Locations creation violates unique constraints (e.g., duplicate names/tags).

#### 2.9.8 Permissions
* `LOCATIONS.VIEW`
* `LOCATIONS.CREATE`
* `LOCATIONS.EDIT`
* `LOCATIONS.DELETE`
* `LOCATIONS.CHECKOUT` (if applicable)

#### 2.9.9 Notifications
* Event `OnLocationsCreated`: Dispatches an informational webhook payload.
* Event `OnLocationsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.9.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Locations records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Locations utilization, depreciation, and assignment history over a given temporal window.

#### 2.9.11 Audit Entries
* Action: `CREATE`, Target: `Locations`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Locations`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Locations`, Payload: Deletion timestamp and initiating actor.

#### 2.9.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Locations.
* Enhanced mobile offline capabilities for modifying Locations in disconnected audit environments.

---

### 2.10 Module: Users

#### 2.10.1 Purpose
The primary purpose of the Users module is to establish strict governance and lifecycle management capabilities. Individuals interacting with the system or holding possession of enterprise assets.

#### 2.10.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Users state changes instantly without page reloads.

#### 2.10.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.10.4 Business Rules
1. **Immutability of History:** Any mutation to a Users record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Users states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Users data must strictly validate against the executing user's Company ID.

#### 2.10.5 Functional Requirements
* **FR-10.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Users records.
* **FR-10.2:** The Angular UI SHALL display a paginated, filterable grid of Users utilizing server-side processing.
* **FR-10.3:** The system SHALL support bulk importing of Users via CSV with pre-flight validation.
* **FR-10.4:** The system SHALL support relational mapping of Users to other enterprise entities (e.g., Users, Locations).

#### 2.10.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.10.7 Error Conditions
* **404 Not Found:** Thrown when a queried Users ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Users creation violates unique constraints (e.g., duplicate names/tags).

#### 2.10.8 Permissions
* `USERS.VIEW`
* `USERS.CREATE`
* `USERS.EDIT`
* `USERS.DELETE`
* `USERS.CHECKOUT` (if applicable)

#### 2.10.9 Notifications
* Event `OnUsersCreated`: Dispatches an informational webhook payload.
* Event `OnUsersAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.10.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Users records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Users utilization, depreciation, and assignment history over a given temporal window.

#### 2.10.11 Audit Entries
* Action: `CREATE`, Target: `Users`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Users`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Users`, Payload: Deletion timestamp and initiating actor.

#### 2.10.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Users.
* Enhanced mobile offline capabilities for modifying Users in disconnected audit environments.

---

### 2.11 Module: Roles

#### 2.11.1 Purpose
The primary purpose of the Roles module is to establish strict governance and lifecycle management capabilities. Pre-defined sets of access levels assigned to users to govern system interaction.

#### 2.11.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Roles state changes instantly without page reloads.

#### 2.11.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.11.4 Business Rules
1. **Immutability of History:** Any mutation to a Roles record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Roles states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Roles data must strictly validate against the executing user's Company ID.

#### 2.11.5 Functional Requirements
* **FR-11.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Roles records.
* **FR-11.2:** The Angular UI SHALL display a paginated, filterable grid of Roles utilizing server-side processing.
* **FR-11.3:** The system SHALL support bulk importing of Roles via CSV with pre-flight validation.
* **FR-11.4:** The system SHALL support relational mapping of Roles to other enterprise entities (e.g., Users, Locations).

#### 2.11.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.11.7 Error Conditions
* **404 Not Found:** Thrown when a queried Roles ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Roles creation violates unique constraints (e.g., duplicate names/tags).

#### 2.11.8 Permissions
* `ROLES.VIEW`
* `ROLES.CREATE`
* `ROLES.EDIT`
* `ROLES.DELETE`
* `ROLES.CHECKOUT` (if applicable)

#### 2.11.9 Notifications
* Event `OnRolesCreated`: Dispatches an informational webhook payload.
* Event `OnRolesAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.11.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Roles records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Roles utilization, depreciation, and assignment history over a given temporal window.

#### 2.11.11 Audit Entries
* Action: `CREATE`, Target: `Roles`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Roles`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Roles`, Payload: Deletion timestamp and initiating actor.

#### 2.11.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Roles.
* Enhanced mobile offline capabilities for modifying Roles in disconnected audit environments.

---

### 2.12 Module: Permissions

#### 2.12.1 Purpose
The primary purpose of the Permissions module is to establish strict governance and lifecycle management capabilities. Granular, bitmask-driven access controls enforcing read, write, and execute restrictions.

#### 2.12.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Permissions state changes instantly without page reloads.

#### 2.12.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.12.4 Business Rules
1. **Immutability of History:** Any mutation to a Permissions record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Permissions states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Permissions data must strictly validate against the executing user's Company ID.

#### 2.12.5 Functional Requirements
* **FR-12.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Permissions records.
* **FR-12.2:** The Angular UI SHALL display a paginated, filterable grid of Permissions utilizing server-side processing.
* **FR-12.3:** The system SHALL support bulk importing of Permissions via CSV with pre-flight validation.
* **FR-12.4:** The system SHALL support relational mapping of Permissions to other enterprise entities (e.g., Users, Locations).

#### 2.12.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.12.7 Error Conditions
* **404 Not Found:** Thrown when a queried Permissions ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Permissions creation violates unique constraints (e.g., duplicate names/tags).

#### 2.12.8 Permissions
* `PERMISSIONS.VIEW`
* `PERMISSIONS.CREATE`
* `PERMISSIONS.EDIT`
* `PERMISSIONS.DELETE`
* `PERMISSIONS.CHECKOUT` (if applicable)

#### 2.12.9 Notifications
* Event `OnPermissionsCreated`: Dispatches an informational webhook payload.
* Event `OnPermissionsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.12.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Permissions records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Permissions utilization, depreciation, and assignment history over a given temporal window.

#### 2.12.11 Audit Entries
* Action: `CREATE`, Target: `Permissions`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Permissions`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Permissions`, Payload: Deletion timestamp and initiating actor.

#### 2.12.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Permissions.
* Enhanced mobile offline capabilities for modifying Permissions in disconnected audit environments.

---

### 2.13 Module: Status Labels

#### 2.13.1 Purpose
The primary purpose of the Status Labels module is to establish strict governance and lifecycle management capabilities. State machine flags (e.g., Deployable, Archived) dictating the permissible lifecycle actions for an asset.

#### 2.13.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Status Labels state changes instantly without page reloads.

#### 2.13.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.13.4 Business Rules
1. **Immutability of History:** Any mutation to a Status Labels record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Status Labels states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Status Labels data must strictly validate against the executing user's Company ID.

#### 2.13.5 Functional Requirements
* **FR-13.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Status Labels records.
* **FR-13.2:** The Angular UI SHALL display a paginated, filterable grid of Status Labels utilizing server-side processing.
* **FR-13.3:** The system SHALL support bulk importing of Status Labels via CSV with pre-flight validation.
* **FR-13.4:** The system SHALL support relational mapping of Status Labels to other enterprise entities (e.g., Users, Locations).

#### 2.13.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.13.7 Error Conditions
* **404 Not Found:** Thrown when a queried Status Labels ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Status Labels creation violates unique constraints (e.g., duplicate names/tags).

#### 2.13.8 Permissions
* `STATUS_LABELS.VIEW`
* `STATUS_LABELS.CREATE`
* `STATUS_LABELS.EDIT`
* `STATUS_LABELS.DELETE`
* `STATUS_LABELS.CHECKOUT` (if applicable)

#### 2.13.9 Notifications
* Event `OnStatusLabelsCreated`: Dispatches an informational webhook payload.
* Event `OnStatusLabelsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.13.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Status Labels records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Status Labels utilization, depreciation, and assignment history over a given temporal window.

#### 2.13.11 Audit Entries
* Action: `CREATE`, Target: `Status Labels`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Status Labels`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Status Labels`, Payload: Deletion timestamp and initiating actor.

#### 2.13.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Status Labels.
* Enhanced mobile offline capabilities for modifying Status Labels in disconnected audit environments.

---

### 2.14 Module: Depreciation Models

#### 2.14.1 Purpose
The primary purpose of the Depreciation Models module is to establish strict governance and lifecycle management capabilities. Financial algorithms (Straight Line, Declining Balance) calculating asset value decay over time.

#### 2.14.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Depreciation Models state changes instantly without page reloads.

#### 2.14.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.14.4 Business Rules
1. **Immutability of History:** Any mutation to a Depreciation Models record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Depreciation Models states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Depreciation Models data must strictly validate against the executing user's Company ID.

#### 2.14.5 Functional Requirements
* **FR-14.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Depreciation Models records.
* **FR-14.2:** The Angular UI SHALL display a paginated, filterable grid of Depreciation Models utilizing server-side processing.
* **FR-14.3:** The system SHALL support bulk importing of Depreciation Models via CSV with pre-flight validation.
* **FR-14.4:** The system SHALL support relational mapping of Depreciation Models to other enterprise entities (e.g., Users, Locations).

#### 2.14.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.14.7 Error Conditions
* **404 Not Found:** Thrown when a queried Depreciation Models ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Depreciation Models creation violates unique constraints (e.g., duplicate names/tags).

#### 2.14.8 Permissions
* `DEPRECIATION_MODELS.VIEW`
* `DEPRECIATION_MODELS.CREATE`
* `DEPRECIATION_MODELS.EDIT`
* `DEPRECIATION_MODELS.DELETE`
* `DEPRECIATION_MODELS.CHECKOUT` (if applicable)

#### 2.14.9 Notifications
* Event `OnDepreciationModelsCreated`: Dispatches an informational webhook payload.
* Event `OnDepreciationModelsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.14.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Depreciation Models records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Depreciation Models utilization, depreciation, and assignment history over a given temporal window.

#### 2.14.11 Audit Entries
* Action: `CREATE`, Target: `Depreciation Models`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Depreciation Models`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Depreciation Models`, Payload: Deletion timestamp and initiating actor.

#### 2.14.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Depreciation Models.
* Enhanced mobile offline capabilities for modifying Depreciation Models in disconnected audit environments.

---

### 2.15 Module: Custom Fields

#### 2.15.1 Purpose
The primary purpose of the Custom Fields module is to establish strict governance and lifecycle management capabilities. User-defined data points extending the baseline schema for specific asset types.

#### 2.15.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Custom Fields state changes instantly without page reloads.

#### 2.15.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.15.4 Business Rules
1. **Immutability of History:** Any mutation to a Custom Fields record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Custom Fields states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Custom Fields data must strictly validate against the executing user's Company ID.

#### 2.15.5 Functional Requirements
* **FR-15.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Custom Fields records.
* **FR-15.2:** The Angular UI SHALL display a paginated, filterable grid of Custom Fields utilizing server-side processing.
* **FR-15.3:** The system SHALL support bulk importing of Custom Fields via CSV with pre-flight validation.
* **FR-15.4:** The system SHALL support relational mapping of Custom Fields to other enterprise entities (e.g., Users, Locations).

#### 2.15.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.15.7 Error Conditions
* **404 Not Found:** Thrown when a queried Custom Fields ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Custom Fields creation violates unique constraints (e.g., duplicate names/tags).

#### 2.15.8 Permissions
* `CUSTOM_FIELDS.VIEW`
* `CUSTOM_FIELDS.CREATE`
* `CUSTOM_FIELDS.EDIT`
* `CUSTOM_FIELDS.DELETE`
* `CUSTOM_FIELDS.CHECKOUT` (if applicable)

#### 2.15.9 Notifications
* Event `OnCustomFieldsCreated`: Dispatches an informational webhook payload.
* Event `OnCustomFieldsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.15.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Custom Fields records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Custom Fields utilization, depreciation, and assignment history over a given temporal window.

#### 2.15.11 Audit Entries
* Action: `CREATE`, Target: `Custom Fields`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Custom Fields`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Custom Fields`, Payload: Deletion timestamp and initiating actor.

#### 2.15.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Custom Fields.
* Enhanced mobile offline capabilities for modifying Custom Fields in disconnected audit environments.

---

### 2.16 Module: Custom Field Sets

#### 2.16.1 Purpose
The primary purpose of the Custom Field Sets module is to establish strict governance and lifecycle management capabilities. Collections of custom fields dynamically bound to asset models.

#### 2.16.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Custom Field Sets state changes instantly without page reloads.

#### 2.16.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.16.4 Business Rules
1. **Immutability of History:** Any mutation to a Custom Field Sets record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Custom Field Sets states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Custom Field Sets data must strictly validate against the executing user's Company ID.

#### 2.16.5 Functional Requirements
* **FR-16.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Custom Field Sets records.
* **FR-16.2:** The Angular UI SHALL display a paginated, filterable grid of Custom Field Sets utilizing server-side processing.
* **FR-16.3:** The system SHALL support bulk importing of Custom Field Sets via CSV with pre-flight validation.
* **FR-16.4:** The system SHALL support relational mapping of Custom Field Sets to other enterprise entities (e.g., Users, Locations).

#### 2.16.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.16.7 Error Conditions
* **404 Not Found:** Thrown when a queried Custom Field Sets ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Custom Field Sets creation violates unique constraints (e.g., duplicate names/tags).

#### 2.16.8 Permissions
* `CUSTOM_FIELD_SETS.VIEW`
* `CUSTOM_FIELD_SETS.CREATE`
* `CUSTOM_FIELD_SETS.EDIT`
* `CUSTOM_FIELD_SETS.DELETE`
* `CUSTOM_FIELD_SETS.CHECKOUT` (if applicable)

#### 2.16.9 Notifications
* Event `OnCustomFieldSetsCreated`: Dispatches an informational webhook payload.
* Event `OnCustomFieldSetsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.16.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Custom Field Sets records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Custom Field Sets utilization, depreciation, and assignment history over a given temporal window.

#### 2.16.11 Audit Entries
* Action: `CREATE`, Target: `Custom Field Sets`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Custom Field Sets`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Custom Field Sets`, Payload: Deletion timestamp and initiating actor.

#### 2.16.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Custom Field Sets.
* Enhanced mobile offline capabilities for modifying Custom Field Sets in disconnected audit environments.

---

### 2.17 Module: Accessories

#### 2.17.1 Purpose
The primary purpose of the Accessories module is to establish strict governance and lifecycle management capabilities. Peripherals assigned to users but not strictly tracked via unique serial numbers (e.g., keyboards).

#### 2.17.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Accessories state changes instantly without page reloads.

#### 2.17.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.17.4 Business Rules
1. **Immutability of History:** Any mutation to a Accessories record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Accessories states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Accessories data must strictly validate against the executing user's Company ID.

#### 2.17.5 Functional Requirements
* **FR-17.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Accessories records.
* **FR-17.2:** The Angular UI SHALL display a paginated, filterable grid of Accessories utilizing server-side processing.
* **FR-17.3:** The system SHALL support bulk importing of Accessories via CSV with pre-flight validation.
* **FR-17.4:** The system SHALL support relational mapping of Accessories to other enterprise entities (e.g., Users, Locations).

#### 2.17.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.17.7 Error Conditions
* **404 Not Found:** Thrown when a queried Accessories ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Accessories creation violates unique constraints (e.g., duplicate names/tags).

#### 2.17.8 Permissions
* `ACCESSORIES.VIEW`
* `ACCESSORIES.CREATE`
* `ACCESSORIES.EDIT`
* `ACCESSORIES.DELETE`
* `ACCESSORIES.CHECKOUT` (if applicable)

#### 2.17.9 Notifications
* Event `OnAccessoriesCreated`: Dispatches an informational webhook payload.
* Event `OnAccessoriesAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.17.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Accessories records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Accessories utilization, depreciation, and assignment history over a given temporal window.

#### 2.17.11 Audit Entries
* Action: `CREATE`, Target: `Accessories`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Accessories`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Accessories`, Payload: Deletion timestamp and initiating actor.

#### 2.17.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Accessories.
* Enhanced mobile offline capabilities for modifying Accessories in disconnected audit environments.

---

### 2.18 Module: Components

#### 2.18.1 Purpose
The primary purpose of the Components module is to establish strict governance and lifecycle management capabilities. Internal parts (e.g., RAM, GPU) that upgrade or repair parent hardware assets.

#### 2.18.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Components state changes instantly without page reloads.

#### 2.18.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.18.4 Business Rules
1. **Immutability of History:** Any mutation to a Components record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Components states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Components data must strictly validate against the executing user's Company ID.

#### 2.18.5 Functional Requirements
* **FR-18.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Components records.
* **FR-18.2:** The Angular UI SHALL display a paginated, filterable grid of Components utilizing server-side processing.
* **FR-18.3:** The system SHALL support bulk importing of Components via CSV with pre-flight validation.
* **FR-18.4:** The system SHALL support relational mapping of Components to other enterprise entities (e.g., Users, Locations).

#### 2.18.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.18.7 Error Conditions
* **404 Not Found:** Thrown when a queried Components ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Components creation violates unique constraints (e.g., duplicate names/tags).

#### 2.18.8 Permissions
* `COMPONENTS.VIEW`
* `COMPONENTS.CREATE`
* `COMPONENTS.EDIT`
* `COMPONENTS.DELETE`
* `COMPONENTS.CHECKOUT` (if applicable)

#### 2.18.9 Notifications
* Event `OnComponentsCreated`: Dispatches an informational webhook payload.
* Event `OnComponentsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.18.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Components records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Components utilization, depreciation, and assignment history over a given temporal window.

#### 2.18.11 Audit Entries
* Action: `CREATE`, Target: `Components`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Components`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Components`, Payload: Deletion timestamp and initiating actor.

#### 2.18.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Components.
* Enhanced mobile offline capabilities for modifying Components in disconnected audit environments.

---

### 2.19 Module: Consumables

#### 2.19.1 Purpose
The primary purpose of the Consumables module is to establish strict governance and lifecycle management capabilities. Depletable inventory items (e.g., printer toner) tracked via FIFO quantity deductions.

#### 2.19.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Consumables state changes instantly without page reloads.

#### 2.19.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.19.4 Business Rules
1. **Immutability of History:** Any mutation to a Consumables record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Consumables states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Consumables data must strictly validate against the executing user's Company ID.

#### 2.19.5 Functional Requirements
* **FR-19.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Consumables records.
* **FR-19.2:** The Angular UI SHALL display a paginated, filterable grid of Consumables utilizing server-side processing.
* **FR-19.3:** The system SHALL support bulk importing of Consumables via CSV with pre-flight validation.
* **FR-19.4:** The system SHALL support relational mapping of Consumables to other enterprise entities (e.g., Users, Locations).

#### 2.19.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.19.7 Error Conditions
* **404 Not Found:** Thrown when a queried Consumables ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Consumables creation violates unique constraints (e.g., duplicate names/tags).

#### 2.19.8 Permissions
* `CONSUMABLES.VIEW`
* `CONSUMABLES.CREATE`
* `CONSUMABLES.EDIT`
* `CONSUMABLES.DELETE`
* `CONSUMABLES.CHECKOUT` (if applicable)

#### 2.19.9 Notifications
* Event `OnConsumablesCreated`: Dispatches an informational webhook payload.
* Event `OnConsumablesAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.19.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Consumables records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Consumables utilization, depreciation, and assignment history over a given temporal window.

#### 2.19.11 Audit Entries
* Action: `CREATE`, Target: `Consumables`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Consumables`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Consumables`, Payload: Deletion timestamp and initiating actor.

#### 2.19.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Consumables.
* Enhanced mobile offline capabilities for modifying Consumables in disconnected audit environments.

---

### 2.20 Module: Software Licenses

#### 2.20.1 Purpose
The primary purpose of the Software Licenses module is to establish strict governance and lifecycle management capabilities. Digital entitlements mapping product keys and available seat counts to physical assets or users.

#### 2.20.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Software Licenses state changes instantly without page reloads.

#### 2.20.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.20.4 Business Rules
1. **Immutability of History:** Any mutation to a Software Licenses record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Software Licenses states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Software Licenses data must strictly validate against the executing user's Company ID.

#### 2.20.5 Functional Requirements
* **FR-20.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Software Licenses records.
* **FR-20.2:** The Angular UI SHALL display a paginated, filterable grid of Software Licenses utilizing server-side processing.
* **FR-20.3:** The system SHALL support bulk importing of Software Licenses via CSV with pre-flight validation.
* **FR-20.4:** The system SHALL support relational mapping of Software Licenses to other enterprise entities (e.g., Users, Locations).

#### 2.20.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.20.7 Error Conditions
* **404 Not Found:** Thrown when a queried Software Licenses ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Software Licenses creation violates unique constraints (e.g., duplicate names/tags).

#### 2.20.8 Permissions
* `SOFTWARE_LICENSES.VIEW`
* `SOFTWARE_LICENSES.CREATE`
* `SOFTWARE_LICENSES.EDIT`
* `SOFTWARE_LICENSES.DELETE`
* `SOFTWARE_LICENSES.CHECKOUT` (if applicable)

#### 2.20.9 Notifications
* Event `OnSoftwareLicensesCreated`: Dispatches an informational webhook payload.
* Event `OnSoftwareLicensesAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.20.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Software Licenses records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Software Licenses utilization, depreciation, and assignment history over a given temporal window.

#### 2.20.11 Audit Entries
* Action: `CREATE`, Target: `Software Licenses`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Software Licenses`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Software Licenses`, Payload: Deletion timestamp and initiating actor.

#### 2.20.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Software Licenses.
* Enhanced mobile offline capabilities for modifying Software Licenses in disconnected audit environments.

---

### 2.21 Module: Maintenance

#### 2.21.1 Purpose
The primary purpose of the Maintenance module is to establish strict governance and lifecycle management capabilities. Scheduled or reactive repair logs detailing service provider interactions and warranty claims.

#### 2.21.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Maintenance state changes instantly without page reloads.

#### 2.21.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.21.4 Business Rules
1. **Immutability of History:** Any mutation to a Maintenance record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Maintenance states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Maintenance data must strictly validate against the executing user's Company ID.

#### 2.21.5 Functional Requirements
* **FR-21.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Maintenance records.
* **FR-21.2:** The Angular UI SHALL display a paginated, filterable grid of Maintenance utilizing server-side processing.
* **FR-21.3:** The system SHALL support bulk importing of Maintenance via CSV with pre-flight validation.
* **FR-21.4:** The system SHALL support relational mapping of Maintenance to other enterprise entities (e.g., Users, Locations).

#### 2.21.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.21.7 Error Conditions
* **404 Not Found:** Thrown when a queried Maintenance ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Maintenance creation violates unique constraints (e.g., duplicate names/tags).

#### 2.21.8 Permissions
* `MAINTENANCE.VIEW`
* `MAINTENANCE.CREATE`
* `MAINTENANCE.EDIT`
* `MAINTENANCE.DELETE`
* `MAINTENANCE.CHECKOUT` (if applicable)

#### 2.21.9 Notifications
* Event `OnMaintenanceCreated`: Dispatches an informational webhook payload.
* Event `OnMaintenanceAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.21.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Maintenance records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Maintenance utilization, depreciation, and assignment history over a given temporal window.

#### 2.21.11 Audit Entries
* Action: `CREATE`, Target: `Maintenance`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Maintenance`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Maintenance`, Payload: Deletion timestamp and initiating actor.

#### 2.21.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Maintenance.
* Enhanced mobile offline capabilities for modifying Maintenance in disconnected audit environments.

---

### 2.22 Module: Reports

#### 2.22.1 Purpose
The primary purpose of the Reports module is to establish strict governance and lifecycle management capabilities. Configurable data extraction utilities generating compliance, financial, and operational audits.

#### 2.22.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Reports state changes instantly without page reloads.

#### 2.22.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.22.4 Business Rules
1. **Immutability of History:** Any mutation to a Reports record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Reports states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Reports data must strictly validate against the executing user's Company ID.

#### 2.22.5 Functional Requirements
* **FR-22.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Reports records.
* **FR-22.2:** The Angular UI SHALL display a paginated, filterable grid of Reports utilizing server-side processing.
* **FR-22.3:** The system SHALL support bulk importing of Reports via CSV with pre-flight validation.
* **FR-22.4:** The system SHALL support relational mapping of Reports to other enterprise entities (e.g., Users, Locations).

#### 2.22.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.22.7 Error Conditions
* **404 Not Found:** Thrown when a queried Reports ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Reports creation violates unique constraints (e.g., duplicate names/tags).

#### 2.22.8 Permissions
* `REPORTS.VIEW`
* `REPORTS.CREATE`
* `REPORTS.EDIT`
* `REPORTS.DELETE`
* `REPORTS.CHECKOUT` (if applicable)

#### 2.22.9 Notifications
* Event `OnReportsCreated`: Dispatches an informational webhook payload.
* Event `OnReportsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.22.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Reports records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Reports utilization, depreciation, and assignment history over a given temporal window.

#### 2.22.11 Audit Entries
* Action: `CREATE`, Target: `Reports`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Reports`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Reports`, Payload: Deletion timestamp and initiating actor.

#### 2.22.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Reports.
* Enhanced mobile offline capabilities for modifying Reports in disconnected audit environments.

---

### 2.23 Module: Notifications

#### 2.23.1 Purpose
The primary purpose of the Notifications module is to establish strict governance and lifecycle management capabilities. Asynchronous messaging engine routing alerts via Email, Slack, Teams, and Webhooks.

#### 2.23.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Notifications state changes instantly without page reloads.

#### 2.23.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.23.4 Business Rules
1. **Immutability of History:** Any mutation to a Notifications record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Notifications states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Notifications data must strictly validate against the executing user's Company ID.

#### 2.23.5 Functional Requirements
* **FR-23.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Notifications records.
* **FR-23.2:** The Angular UI SHALL display a paginated, filterable grid of Notifications utilizing server-side processing.
* **FR-23.3:** The system SHALL support bulk importing of Notifications via CSV with pre-flight validation.
* **FR-23.4:** The system SHALL support relational mapping of Notifications to other enterprise entities (e.g., Users, Locations).

#### 2.23.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.23.7 Error Conditions
* **404 Not Found:** Thrown when a queried Notifications ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Notifications creation violates unique constraints (e.g., duplicate names/tags).

#### 2.23.8 Permissions
* `NOTIFICATIONS.VIEW`
* `NOTIFICATIONS.CREATE`
* `NOTIFICATIONS.EDIT`
* `NOTIFICATIONS.DELETE`
* `NOTIFICATIONS.CHECKOUT` (if applicable)

#### 2.23.9 Notifications
* Event `OnNotificationsCreated`: Dispatches an informational webhook payload.
* Event `OnNotificationsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.23.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Notifications records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Notifications utilization, depreciation, and assignment history over a given temporal window.

#### 2.23.11 Audit Entries
* Action: `CREATE`, Target: `Notifications`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Notifications`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Notifications`, Payload: Deletion timestamp and initiating actor.

#### 2.23.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Notifications.
* Enhanced mobile offline capabilities for modifying Notifications in disconnected audit environments.

---

### 2.24 Module: Search

#### 2.24.1 Purpose
The primary purpose of the Search module is to establish strict governance and lifecycle management capabilities. Global indexing engine providing low-latency, full-text retrieval across all system entities.

#### 2.24.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Search state changes instantly without page reloads.

#### 2.24.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.24.4 Business Rules
1. **Immutability of History:** Any mutation to a Search record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Search states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Search data must strictly validate against the executing user's Company ID.

#### 2.24.5 Functional Requirements
* **FR-24.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Search records.
* **FR-24.2:** The Angular UI SHALL display a paginated, filterable grid of Search utilizing server-side processing.
* **FR-24.3:** The system SHALL support bulk importing of Search via CSV with pre-flight validation.
* **FR-24.4:** The system SHALL support relational mapping of Search to other enterprise entities (e.g., Users, Locations).

#### 2.24.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.24.7 Error Conditions
* **404 Not Found:** Thrown when a queried Search ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Search creation violates unique constraints (e.g., duplicate names/tags).

#### 2.24.8 Permissions
* `SEARCH.VIEW`
* `SEARCH.CREATE`
* `SEARCH.EDIT`
* `SEARCH.DELETE`
* `SEARCH.CHECKOUT` (if applicable)

#### 2.24.9 Notifications
* Event `OnSearchCreated`: Dispatches an informational webhook payload.
* Event `OnSearchAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.24.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Search records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Search utilization, depreciation, and assignment history over a given temporal window.

#### 2.24.11 Audit Entries
* Action: `CREATE`, Target: `Search`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Search`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Search`, Payload: Deletion timestamp and initiating actor.

#### 2.24.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Search.
* Enhanced mobile offline capabilities for modifying Search in disconnected audit environments.

---

### 2.25 Module: Saved Searches

#### 2.25.1 Purpose
The primary purpose of the Saved Searches module is to establish strict governance and lifecycle management capabilities. Persisted query configurations utilized for dynamic reporting and automated email alerts.

#### 2.25.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Saved Searches state changes instantly without page reloads.

#### 2.25.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.25.4 Business Rules
1. **Immutability of History:** Any mutation to a Saved Searches record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Saved Searches states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Saved Searches data must strictly validate against the executing user's Company ID.

#### 2.25.5 Functional Requirements
* **FR-25.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Saved Searches records.
* **FR-25.2:** The Angular UI SHALL display a paginated, filterable grid of Saved Searches utilizing server-side processing.
* **FR-25.3:** The system SHALL support bulk importing of Saved Searches via CSV with pre-flight validation.
* **FR-25.4:** The system SHALL support relational mapping of Saved Searches to other enterprise entities (e.g., Users, Locations).

#### 2.25.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.25.7 Error Conditions
* **404 Not Found:** Thrown when a queried Saved Searches ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Saved Searches creation violates unique constraints (e.g., duplicate names/tags).

#### 2.25.8 Permissions
* `SAVED_SEARCHES.VIEW`
* `SAVED_SEARCHES.CREATE`
* `SAVED_SEARCHES.EDIT`
* `SAVED_SEARCHES.DELETE`
* `SAVED_SEARCHES.CHECKOUT` (if applicable)

#### 2.25.9 Notifications
* Event `OnSavedSearchesCreated`: Dispatches an informational webhook payload.
* Event `OnSavedSearchesAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.25.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Saved Searches records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Saved Searches utilization, depreciation, and assignment history over a given temporal window.

#### 2.25.11 Audit Entries
* Action: `CREATE`, Target: `Saved Searches`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Saved Searches`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Saved Searches`, Payload: Deletion timestamp and initiating actor.

#### 2.25.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Saved Searches.
* Enhanced mobile offline capabilities for modifying Saved Searches in disconnected audit environments.

---

### 2.26 Module: Import

#### 2.26.1 Purpose
The primary purpose of the Import module is to establish strict governance and lifecycle management capabilities. Bulk data ingestion pipelines validating and transforming CSV/JSON payloads into domain entities.

#### 2.26.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Import state changes instantly without page reloads.

#### 2.26.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.26.4 Business Rules
1. **Immutability of History:** Any mutation to a Import record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Import states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Import data must strictly validate against the executing user's Company ID.

#### 2.26.5 Functional Requirements
* **FR-26.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Import records.
* **FR-26.2:** The Angular UI SHALL display a paginated, filterable grid of Import utilizing server-side processing.
* **FR-26.3:** The system SHALL support bulk importing of Import via CSV with pre-flight validation.
* **FR-26.4:** The system SHALL support relational mapping of Import to other enterprise entities (e.g., Users, Locations).

#### 2.26.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.26.7 Error Conditions
* **404 Not Found:** Thrown when a queried Import ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Import creation violates unique constraints (e.g., duplicate names/tags).

#### 2.26.8 Permissions
* `IMPORT.VIEW`
* `IMPORT.CREATE`
* `IMPORT.EDIT`
* `IMPORT.DELETE`
* `IMPORT.CHECKOUT` (if applicable)

#### 2.26.9 Notifications
* Event `OnImportCreated`: Dispatches an informational webhook payload.
* Event `OnImportAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.26.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Import records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Import utilization, depreciation, and assignment history over a given temporal window.

#### 2.26.11 Audit Entries
* Action: `CREATE`, Target: `Import`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Import`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Import`, Payload: Deletion timestamp and initiating actor.

#### 2.26.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Import.
* Enhanced mobile offline capabilities for modifying Import in disconnected audit environments.

---

### 2.27 Module: Export

#### 2.27.1 Purpose
The primary purpose of the Export module is to establish strict governance and lifecycle management capabilities. Data egress handlers serializing grid views and reports into standard file formats.

#### 2.27.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Export state changes instantly without page reloads.

#### 2.27.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.27.4 Business Rules
1. **Immutability of History:** Any mutation to a Export record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Export states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Export data must strictly validate against the executing user's Company ID.

#### 2.27.5 Functional Requirements
* **FR-27.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Export records.
* **FR-27.2:** The Angular UI SHALL display a paginated, filterable grid of Export utilizing server-side processing.
* **FR-27.3:** The system SHALL support bulk importing of Export via CSV with pre-flight validation.
* **FR-27.4:** The system SHALL support relational mapping of Export to other enterprise entities (e.g., Users, Locations).

#### 2.27.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.27.7 Error Conditions
* **404 Not Found:** Thrown when a queried Export ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Export creation violates unique constraints (e.g., duplicate names/tags).

#### 2.27.8 Permissions
* `EXPORT.VIEW`
* `EXPORT.CREATE`
* `EXPORT.EDIT`
* `EXPORT.DELETE`
* `EXPORT.CHECKOUT` (if applicable)

#### 2.27.9 Notifications
* Event `OnExportCreated`: Dispatches an informational webhook payload.
* Event `OnExportAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.27.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Export records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Export utilization, depreciation, and assignment history over a given temporal window.

#### 2.27.11 Audit Entries
* Action: `CREATE`, Target: `Export`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Export`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Export`, Payload: Deletion timestamp and initiating actor.

#### 2.27.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Export.
* Enhanced mobile offline capabilities for modifying Export in disconnected audit environments.

---

### 2.28 Module: Barcode

#### 2.28.1 Purpose
The primary purpose of the Barcode module is to establish strict governance and lifecycle management capabilities. Symbology generation (1D) and parsing modules for rapid physical asset identification.

#### 2.28.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Barcode state changes instantly without page reloads.

#### 2.28.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.28.4 Business Rules
1. **Immutability of History:** Any mutation to a Barcode record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Barcode states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Barcode data must strictly validate against the executing user's Company ID.

#### 2.28.5 Functional Requirements
* **FR-28.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Barcode records.
* **FR-28.2:** The Angular UI SHALL display a paginated, filterable grid of Barcode utilizing server-side processing.
* **FR-28.3:** The system SHALL support bulk importing of Barcode via CSV with pre-flight validation.
* **FR-28.4:** The system SHALL support relational mapping of Barcode to other enterprise entities (e.g., Users, Locations).

#### 2.28.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.28.7 Error Conditions
* **404 Not Found:** Thrown when a queried Barcode ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Barcode creation violates unique constraints (e.g., duplicate names/tags).

#### 2.28.8 Permissions
* `BARCODE.VIEW`
* `BARCODE.CREATE`
* `BARCODE.EDIT`
* `BARCODE.DELETE`
* `BARCODE.CHECKOUT` (if applicable)

#### 2.28.9 Notifications
* Event `OnBarcodeCreated`: Dispatches an informational webhook payload.
* Event `OnBarcodeAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.28.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Barcode records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Barcode utilization, depreciation, and assignment history over a given temporal window.

#### 2.28.11 Audit Entries
* Action: `CREATE`, Target: `Barcode`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Barcode`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Barcode`, Payload: Deletion timestamp and initiating actor.

#### 2.28.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Barcode.
* Enhanced mobile offline capabilities for modifying Barcode in disconnected audit environments.

---

### 2.29 Module: QR Code

#### 2.29.1 Purpose
The primary purpose of the QR Code module is to establish strict governance and lifecycle management capabilities. 2D barcode generation linking directly to the mobile-optimized asset detail URI.

#### 2.29.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect QR Code state changes instantly without page reloads.

#### 2.29.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.29.4 Business Rules
1. **Immutability of History:** Any mutation to a QR Code record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning QR Code states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, QR Code data must strictly validate against the executing user's Company ID.

#### 2.29.5 Functional Requirements
* **FR-29.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete QR Code records.
* **FR-29.2:** The Angular UI SHALL display a paginated, filterable grid of QR Code utilizing server-side processing.
* **FR-29.3:** The system SHALL support bulk importing of QR Code via CSV with pre-flight validation.
* **FR-29.4:** The system SHALL support relational mapping of QR Code to other enterprise entities (e.g., Users, Locations).

#### 2.29.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.29.7 Error Conditions
* **404 Not Found:** Thrown when a queried QR Code ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when QR Code creation violates unique constraints (e.g., duplicate names/tags).

#### 2.29.8 Permissions
* `QR_CODE.VIEW`
* `QR_CODE.CREATE`
* `QR_CODE.EDIT`
* `QR_CODE.DELETE`
* `QR_CODE.CHECKOUT` (if applicable)

#### 2.29.9 Notifications
* Event `OnQRCodeCreated`: Dispatches an informational webhook payload.
* Event `OnQRCodeAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.29.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active QR Code records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of QR Code utilization, depreciation, and assignment history over a given temporal window.

#### 2.29.11 Audit Entries
* Action: `CREATE`, Target: `QR Code`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `QR Code`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `QR Code`, Payload: Deletion timestamp and initiating actor.

#### 2.29.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding QR Code.
* Enhanced mobile offline capabilities for modifying QR Code in disconnected audit environments.

---

### 2.30 Module: Activity Logs

#### 2.30.1 Purpose
The primary purpose of the Activity Logs module is to establish strict governance and lifecycle management capabilities. Immutable, temporal transaction ledgers recording all state mutations and access events.

#### 2.30.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Activity Logs state changes instantly without page reloads.

#### 2.30.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.30.4 Business Rules
1. **Immutability of History:** Any mutation to a Activity Logs record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Activity Logs states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Activity Logs data must strictly validate against the executing user's Company ID.

#### 2.30.5 Functional Requirements
* **FR-30.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Activity Logs records.
* **FR-30.2:** The Angular UI SHALL display a paginated, filterable grid of Activity Logs utilizing server-side processing.
* **FR-30.3:** The system SHALL support bulk importing of Activity Logs via CSV with pre-flight validation.
* **FR-30.4:** The system SHALL support relational mapping of Activity Logs to other enterprise entities (e.g., Users, Locations).

#### 2.30.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.30.7 Error Conditions
* **404 Not Found:** Thrown when a queried Activity Logs ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Activity Logs creation violates unique constraints (e.g., duplicate names/tags).

#### 2.30.8 Permissions
* `ACTIVITY_LOGS.VIEW`
* `ACTIVITY_LOGS.CREATE`
* `ACTIVITY_LOGS.EDIT`
* `ACTIVITY_LOGS.DELETE`
* `ACTIVITY_LOGS.CHECKOUT` (if applicable)

#### 2.30.9 Notifications
* Event `OnActivityLogsCreated`: Dispatches an informational webhook payload.
* Event `OnActivityLogsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.30.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Activity Logs records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Activity Logs utilization, depreciation, and assignment history over a given temporal window.

#### 2.30.11 Audit Entries
* Action: `CREATE`, Target: `Activity Logs`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Activity Logs`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Activity Logs`, Payload: Deletion timestamp and initiating actor.

#### 2.30.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Activity Logs.
* Enhanced mobile offline capabilities for modifying Activity Logs in disconnected audit environments.

---

### 2.31 Module: Settings

#### 2.31.1 Purpose
The primary purpose of the Settings module is to establish strict governance and lifecycle management capabilities. Global application configuration toggles governing security, localization, and integration behaviors.

#### 2.31.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Settings state changes instantly without page reloads.

#### 2.31.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.31.4 Business Rules
1. **Immutability of History:** Any mutation to a Settings record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Settings states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Settings data must strictly validate against the executing user's Company ID.

#### 2.31.5 Functional Requirements
* **FR-31.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Settings records.
* **FR-31.2:** The Angular UI SHALL display a paginated, filterable grid of Settings utilizing server-side processing.
* **FR-31.3:** The system SHALL support bulk importing of Settings via CSV with pre-flight validation.
* **FR-31.4:** The system SHALL support relational mapping of Settings to other enterprise entities (e.g., Users, Locations).

#### 2.31.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.31.7 Error Conditions
* **404 Not Found:** Thrown when a queried Settings ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Settings creation violates unique constraints (e.g., duplicate names/tags).

#### 2.31.8 Permissions
* `SETTINGS.VIEW`
* `SETTINGS.CREATE`
* `SETTINGS.EDIT`
* `SETTINGS.DELETE`
* `SETTINGS.CHECKOUT` (if applicable)

#### 2.31.9 Notifications
* Event `OnSettingsCreated`: Dispatches an informational webhook payload.
* Event `OnSettingsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.31.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Settings records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Settings utilization, depreciation, and assignment history over a given temporal window.

#### 2.31.11 Audit Entries
* Action: `CREATE`, Target: `Settings`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Settings`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Settings`, Payload: Deletion timestamp and initiating actor.

#### 2.31.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Settings.
* Enhanced mobile offline capabilities for modifying Settings in disconnected audit environments.

---

### 2.32 Module: Localization

#### 2.32.1 Purpose
The primary purpose of the Localization module is to establish strict governance and lifecycle management capabilities. Internationalization (i18n) engine serving contextual language translations and regional formats.

#### 2.32.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Localization state changes instantly without page reloads.

#### 2.32.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.32.4 Business Rules
1. **Immutability of History:** Any mutation to a Localization record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Localization states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Localization data must strictly validate against the executing user's Company ID.

#### 2.32.5 Functional Requirements
* **FR-32.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Localization records.
* **FR-32.2:** The Angular UI SHALL display a paginated, filterable grid of Localization utilizing server-side processing.
* **FR-32.3:** The system SHALL support bulk importing of Localization via CSV with pre-flight validation.
* **FR-32.4:** The system SHALL support relational mapping of Localization to other enterprise entities (e.g., Users, Locations).

#### 2.32.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.32.7 Error Conditions
* **404 Not Found:** Thrown when a queried Localization ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Localization creation violates unique constraints (e.g., duplicate names/tags).

#### 2.32.8 Permissions
* `LOCALIZATION.VIEW`
* `LOCALIZATION.CREATE`
* `LOCALIZATION.EDIT`
* `LOCALIZATION.DELETE`
* `LOCALIZATION.CHECKOUT` (if applicable)

#### 2.32.9 Notifications
* Event `OnLocalizationCreated`: Dispatches an informational webhook payload.
* Event `OnLocalizationAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.32.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Localization records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Localization utilization, depreciation, and assignment history over a given temporal window.

#### 2.32.11 Audit Entries
* Action: `CREATE`, Target: `Localization`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Localization`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Localization`, Payload: Deletion timestamp and initiating actor.

#### 2.32.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Localization.
* Enhanced mobile offline capabilities for modifying Localization in disconnected audit environments.

---

### 2.33 Module: Email Templates

#### 2.33.1 Purpose
The primary purpose of the Email Templates module is to establish strict governance and lifecycle management capabilities. Customizable HTML notification structures driven by variable substitution.

#### 2.33.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Email Templates state changes instantly without page reloads.

#### 2.33.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.33.4 Business Rules
1. **Immutability of History:** Any mutation to a Email Templates record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Email Templates states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Email Templates data must strictly validate against the executing user's Company ID.

#### 2.33.5 Functional Requirements
* **FR-33.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Email Templates records.
* **FR-33.2:** The Angular UI SHALL display a paginated, filterable grid of Email Templates utilizing server-side processing.
* **FR-33.3:** The system SHALL support bulk importing of Email Templates via CSV with pre-flight validation.
* **FR-33.4:** The system SHALL support relational mapping of Email Templates to other enterprise entities (e.g., Users, Locations).

#### 2.33.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.33.7 Error Conditions
* **404 Not Found:** Thrown when a queried Email Templates ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Email Templates creation violates unique constraints (e.g., duplicate names/tags).

#### 2.33.8 Permissions
* `EMAIL_TEMPLATES.VIEW`
* `EMAIL_TEMPLATES.CREATE`
* `EMAIL_TEMPLATES.EDIT`
* `EMAIL_TEMPLATES.DELETE`
* `EMAIL_TEMPLATES.CHECKOUT` (if applicable)

#### 2.33.9 Notifications
* Event `OnEmailTemplatesCreated`: Dispatches an informational webhook payload.
* Event `OnEmailTemplatesAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.33.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Email Templates records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Email Templates utilization, depreciation, and assignment history over a given temporal window.

#### 2.33.11 Audit Entries
* Action: `CREATE`, Target: `Email Templates`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Email Templates`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Email Templates`, Payload: Deletion timestamp and initiating actor.

#### 2.33.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Email Templates.
* Enhanced mobile offline capabilities for modifying Email Templates in disconnected audit environments.

---

### 2.34 Module: API Tokens

#### 2.34.1 Purpose
The primary purpose of the API Tokens module is to establish strict governance and lifecycle management capabilities. Cryptographic bearer tokens authenticating headless integrations and third-party services.

#### 2.34.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect API Tokens state changes instantly without page reloads.

#### 2.34.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.34.4 Business Rules
1. **Immutability of History:** Any mutation to a API Tokens record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning API Tokens states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, API Tokens data must strictly validate against the executing user's Company ID.

#### 2.34.5 Functional Requirements
* **FR-34.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete API Tokens records.
* **FR-34.2:** The Angular UI SHALL display a paginated, filterable grid of API Tokens utilizing server-side processing.
* **FR-34.3:** The system SHALL support bulk importing of API Tokens via CSV with pre-flight validation.
* **FR-34.4:** The system SHALL support relational mapping of API Tokens to other enterprise entities (e.g., Users, Locations).

#### 2.34.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.34.7 Error Conditions
* **404 Not Found:** Thrown when a queried API Tokens ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when API Tokens creation violates unique constraints (e.g., duplicate names/tags).

#### 2.34.8 Permissions
* `API_TOKENS.VIEW`
* `API_TOKENS.CREATE`
* `API_TOKENS.EDIT`
* `API_TOKENS.DELETE`
* `API_TOKENS.CHECKOUT` (if applicable)

#### 2.34.9 Notifications
* Event `OnAPITokensCreated`: Dispatches an informational webhook payload.
* Event `OnAPITokensAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.34.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active API Tokens records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of API Tokens utilization, depreciation, and assignment history over a given temporal window.

#### 2.34.11 Audit Entries
* Action: `CREATE`, Target: `API Tokens`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `API Tokens`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `API Tokens`, Payload: Deletion timestamp and initiating actor.

#### 2.34.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding API Tokens.
* Enhanced mobile offline capabilities for modifying API Tokens in disconnected audit environments.

---

### 2.35 Module: Attachments

#### 2.35.1 Purpose
The primary purpose of the Attachments module is to establish strict governance and lifecycle management capabilities. Binary blobs (PDFs, images) securely associated with domain entities via Azure Blob/S3 storage interfaces.

#### 2.35.2 Overview
This module operates within the CQRS boundary, accepting commands for state mutation and exposing optimized read models. It interfaces directly with the SQL Server temporal tables to ensure absolute historical accuracy. The Angular 20+ frontend utilizes Signals to reflect Attachments state changes instantly without page reloads.

#### 2.35.3 Actors
* **Super Administrator:** Full CRUD and configuration privileges.
* **Location IT Manager:** Scoped CRUD operations restricted by Row-Level Security (RLS) to their designated department/location.
* **IT Support Technician:** Execution of operational workflows (Check-in, Check-out, Audit) regarding this module.
* **System (Automated):** Background workers triggering automated state transitions and audit logs.

#### 2.35.4 Business Rules
1. **Immutability of History:** Any mutation to a Attachments record must immediately generate a cryptographically verifiable entry in the Activity Logs.
2. **State Machine Conformance:** Transitioning Attachments states must adhere to the defined meta-status constraints (e.g., deployed entities cannot be soft-deleted).
3. **Tenant Isolation:** In multi-tenant modes, Attachments data must strictly validate against the executing user's Company ID.

#### 2.35.5 Functional Requirements
* **FR-35.1:** The system SHALL provide an API endpoint to create, read, update, and soft-delete Attachments records.
* **FR-35.2:** The Angular UI SHALL display a paginated, filterable grid of Attachments utilizing server-side processing.
* **FR-35.3:** The system SHALL support bulk importing of Attachments via CSV with pre-flight validation.
* **FR-35.4:** The system SHALL support relational mapping of Attachments to other enterprise entities (e.g., Users, Locations).

#### 2.35.6 Validation Rules
* Mandatory fields (Name, Associated Company, Status) must be enforced via FluentValidation pipelines.
* String length constraints: Names (max 255 chars), Notes (max 2000 chars).
* Referential integrity: Foreign keys (e.g., assigned User ID) must exist and be active within the database.

#### 2.35.7 Error Conditions
* **404 Not Found:** Thrown when a queried Attachments ID does not exist or falls outside the user's RLS scope.
* **409 Conflict:** Thrown during concurrent update attempts where the `RowVersion` token mismatches (Optimistic Concurrency).
* **422 Unprocessable Entity:** Thrown when Attachments creation violates unique constraints (e.g., duplicate names/tags).

#### 2.35.8 Permissions
* `ATTACHMENTS.VIEW`
* `ATTACHMENTS.CREATE`
* `ATTACHMENTS.EDIT`
* `ATTACHMENTS.DELETE`
* `ATTACHMENTS.CHECKOUT` (if applicable)

#### 2.35.9 Notifications
* Event `OnAttachmentsCreated`: Dispatches an informational webhook payload.
* Event `OnAttachmentsAssigned`: Triggers standard EULA acceptance email workflows to the target user.

#### 2.35.10 Reports
* **Standard Listing Report:** CSV/PDF export of all active Attachments records matching current UI filters.
* **Lifecycle Analytics:** Aggregated views of Attachments utilization, depreciation, and assignment history over a given temporal window.

#### 2.35.11 Audit Entries
* Action: `CREATE`, Target: `Attachments`, Payload: Initial JSON state.
* Action: `UPDATE`, Target: `Attachments`, Payload: JSON diff of modified properties.
* Action: `DELETE`, Target: `Attachments`, Payload: Deletion timestamp and initiating actor.

#### 2.35.12 Future Enhancements
* Integration with internal AI agentic workflows (Tracer-AI) for predictive lifecycle recommendations regarding Attachments.
* Enhanced mobile offline capabilities for modifying Attachments in disconnected audit environments.

---

## 3. Global Cross-Cutting Concerns
### 3.1 Concurrency Handling
Optimistic concurrency control is implemented utilizing Entity Framework Core `RowVersion` tokens across all aggregates to prevent lost updates.

### 3.2 Pagination & Sorting
All read lists implement cursor-based or keyset pagination to maintain sub-100ms response times on datasets exceeding 1,000,000 rows.

### 3.3 Extensibility
The system architecture guarantees that new domain entities can be introduced by adhering to the `IAggregateRoot` interfaces without breaking existing MediatR command pathways.

*End of Document 2. Awaiting next instruction.*
