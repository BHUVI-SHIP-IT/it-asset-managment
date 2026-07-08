IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Companies] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [Name] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
);

CREATE TABLE [OutboxMessages] (
    [Id] uniqueidentifier NOT NULL,
    [Type] nvarchar(512) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [OccurredOnUtc] datetime2 NOT NULL,
    [ProcessedOnUtc] datetime2 NULL,
    [Error] nvarchar(4000) NULL,
    CONSTRAINT [PK_OutboxMessages] PRIMARY KEY ([Id])
);

CREATE TABLE [Permissions] (
    [Id] int NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);

CREATE TABLE [Roles] (
    [Id] int NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [StatusLabels] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [IsDeployable] bit NOT NULL,
    [IsPending] bit NOT NULL,
    [IsArchived] bit NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] varbinary(max) NOT NULL,
    CONSTRAINT [PK_StatusLabels] PRIMARY KEY ([Id])
);

CREATE TABLE [Categories] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] varbinary(max) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Departments] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] varbinary(max) NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Departments_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Locations] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] varbinary(max) NOT NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Locations_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Manufacturers] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] varbinary(max) NOT NULL,
    CONSTRAINT [PK_Manufacturers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Manufacturers_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Suppliers] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] varbinary(max) NOT NULL,
    CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Suppliers_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [RolePermissions] (
    [RoleId] int NOT NULL,
    [PermissionId] int NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
    CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [FullName] nvarchar(255) NOT NULL,
    [Email] nvarchar(320) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [IsActive] bit NOT NULL,
    [LastLoginAtUtc] datetime2 NULL,
    [RefreshToken] nvarchar(100) NULL,
    [RefreshTokenExpiryUtc] datetime2 NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [AssetModels] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [ManufacturerId] uniqueidentifier NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] varbinary(max) NOT NULL,
    CONSTRAINT [PK_AssetModels] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AssetModels_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AssetModels_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AssetModels_Manufacturers_ManufacturerId] FOREIGN KEY ([ManufacturerId]) REFERENCES [Manufacturers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Assets] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [AssetTag] nvarchar(255) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [SerialNumber] nvarchar(255) NULL,
    [Status] int NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [AssetModelId] uniqueidentifier NOT NULL,
    [StatusLabelId] int NOT NULL,
    [LocationId] uniqueidentifier NULL,
    [AssignedUserId] uniqueidentifier NULL,
    [CheckedOutAtUtc] datetime2 NULL,
    [LastCheckinAtUtc] datetime2 NULL,
    [PurchaseCost] decimal(18,2) NOT NULL,
    [PurchaseDate] datetime2 NULL,
    [Notes] nvarchar(4000) NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_Assets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Assets_AssetModels_AssetModelId] FOREIGN KEY ([AssetModelId]) REFERENCES [AssetModels] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Assets_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Assets_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Assets_StatusLabels_StatusLabelId] FOREIGN KEY ([StatusLabelId]) REFERENCES [StatusLabels] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Assets_Users_AssignedUserId] FOREIGN KEY ([AssignedUserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Companies]'))
    SET IDENTITY_INSERT [Companies] ON;
INSERT INTO [Companies] ([Id], [Name])
VALUES ('00000000-0000-0000-0000-000000000001', N'Tracer Headquarters');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Companies]'))
    SET IDENTITY_INSERT [Companies] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([Id], [Description], [Name])
VALUES (1, N'Allows Assets.View', N'Assets.View'),
(2, N'Allows Assets.Create', N'Assets.Create'),
(3, N'Allows Assets.Edit', N'Assets.Edit'),
(4, N'Allows Assets.Delete', N'Assets.Delete'),
(5, N'Allows Assets.Assign', N'Assets.Assign'),
(6, N'Allows Assets.CheckOut', N'Assets.CheckOut'),
(7, N'Allows Assets.CheckIn', N'Assets.CheckIn'),
(8, N'Allows Assets.Transfer', N'Assets.Transfer'),
(9, N'Allows Assets.Clone', N'Assets.Clone'),
(10, N'Allows Assets.Dispose', N'Assets.Dispose'),
(11, N'Allows Assets.Archive', N'Assets.Archive'),
(12, N'Allows Users.View', N'Users.View'),
(13, N'Allows Users.Create', N'Users.Create'),
(14, N'Allows Users.Edit', N'Users.Edit'),
(15, N'Allows Users.Delete', N'Users.Delete'),
(16, N'Allows Roles.Manage', N'Roles.Manage'),
(17, N'Allows Permissions.Manage', N'Permissions.Manage'),
(18, N'Allows Reports.View', N'Reports.View'),
(19, N'Allows Reports.Export', N'Reports.Export'),
(20, N'Allows Settings.Manage', N'Settings.Manage'),
(21, N'Allows API.Manage', N'API.Manage'),
(22, N'Allows Notifications.Manage', N'Notifications.Manage'),
(23, N'Allows Maintenance.Manage', N'Maintenance.Manage'),
(24, N'Allows AuditLogs.View', N'AuditLogs.View'),
(25, N'Allows Licenses.Manage', N'Licenses.Manage'),
(26, N'Allows Accessories.Manage', N'Accessories.Manage'),
(27, N'Allows Components.Manage', N'Components.Manage'),
(28, N'Allows Consumables.Manage', N'Consumables.Manage');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [Description], [Name])
VALUES (1, N'Ultimate authority over the Tracer system', N'SuperAdmin'),
(2, N'Technical management of ITAM application', N'SystemAdmin'),
(3, N'Oversight of IT assets and workflows', N'ITAdmin'),
(4, N'Strategic management of asset portfolio', N'AssetManager'),
(5, N'Daily physical management of inventory', N'InventoryManager'),
(6, N'Managing supplier and inbound purchases', N'ProcurementOfficer'),
(7, N'Oversight of departmental assets', N'DepartmentManager'),
(8, N'Managing financial lifecycle of assets', N'FinanceOfficer'),
(9, N'Independent verification of compliance', N'Auditor'),
(10, N'Frontline support managing deployments', N'HelpDesk'),
(11, N'Standard end-user', N'Employee'),
(12, N'Unauthenticated or restricted access', N'Guest');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'IsArchived', N'IsDeleted', N'IsDeployable', N'IsPending', N'Name', N'RowVersion', N'UpdatedAtUtc', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[StatusLabels]'))
    SET IDENTITY_INSERT [StatusLabels] ON;
INSERT INTO [StatusLabels] ([Id], [CreatedAtUtc], [CreatedBy], [DeletedAtUtc], [IsArchived], [IsDeleted], [IsDeployable], [IsPending], [Name], [RowVersion], [UpdatedAtUtc], [UpdatedBy])
VALUES (1, '0001-01-01T00:00:00.0000000', NULL, NULL, CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Deployable', 0x, NULL, NULL),
(2, '0001-01-01T00:00:00.0000000', NULL, NULL, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Deployed', 0x, NULL, NULL),
(3, '0001-01-01T00:00:00.0000000', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Archived', 0x, NULL, NULL),
(4, '0001-01-01T00:00:00.0000000', NULL, NULL, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Broken', 0x, NULL, NULL),
(5, '0001-01-01T00:00:00.0000000', NULL, NULL, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Pending', 0x, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'IsArchived', N'IsDeleted', N'IsDeployable', N'IsPending', N'Name', N'RowVersion', N'UpdatedAtUtc', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[StatusLabels]'))
    SET IDENTITY_INSERT [StatusLabels] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] ON;
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (1, 1),
(2, 1),
(3, 1),
(4, 1),
(5, 1),
(6, 1),
(7, 1),
(8, 1),
(9, 1),
(10, 1),
(11, 1),
(12, 1),
(13, 1),
(14, 1),
(15, 1),
(16, 1),
(17, 1),
(18, 1),
(19, 1),
(20, 1),
(21, 1),
(22, 1),
(23, 1),
(24, 1),
(25, 1),
(26, 1),
(27, 1),
(28, 1),
(1, 5),
(2, 5),
(3, 5),
(6, 5),
(7, 5),
(1, 10),
(6, 10),
(7, 10),
(12, 10),
(23, 10);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'Email', N'FullName', N'IsActive', N'LastLoginAtUtc', N'PasswordHash', N'RefreshToken', N'RefreshTokenExpiryUtc', N'RoleId') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [CompanyId], [Email], [FullName], [IsActive], [LastLoginAtUtc], [PasswordHash], [RefreshToken], [RefreshTokenExpiryUtc], [RoleId])
VALUES ('11111111-1111-1111-1111-111111111111', '00000000-0000-0000-0000-000000000001', N'admin@tracer.io', N'System Administrator', CAST(1 AS bit), NULL, N'$2a$11$N9V2V2W41q4.F854hV5/Z.tJjU.n/q.4mO1h3Z/g71.p3z7g91/m6', NULL, NULL, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'Email', N'FullName', N'IsActive', N'LastLoginAtUtc', N'PasswordHash', N'RefreshToken', N'RefreshTokenExpiryUtc', N'RoleId') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;

CREATE INDEX [IX_AssetModels_CategoryId] ON [AssetModels] ([CategoryId]);

CREATE INDEX [IX_AssetModels_CompanyId] ON [AssetModels] ([CompanyId]);

CREATE INDEX [IX_AssetModels_ManufacturerId] ON [AssetModels] ([ManufacturerId]);

CREATE UNIQUE INDEX [IX_AssetModels_Name] ON [AssetModels] ([Name]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_Assets_AssetModelId] ON [Assets] ([AssetModelId]);

CREATE INDEX [IX_Assets_AssignedUserId] ON [Assets] ([AssignedUserId]);

CREATE INDEX [IX_Assets_CompanyId_StatusLabelId] ON [Assets] ([CompanyId], [StatusLabelId]);

CREATE INDEX [IX_Assets_LocationId] ON [Assets] ([LocationId]);

CREATE INDEX [IX_Assets_StatusLabelId] ON [Assets] ([StatusLabelId]);

CREATE UNIQUE INDEX [UX_Assets_CompanyId_AssetTag] ON [Assets] ([CompanyId], [AssetTag]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_Categories_CompanyId] ON [Categories] ([CompanyId]);

CREATE UNIQUE INDEX [IX_Categories_Name] ON [Categories] ([Name]) WHERE [IsDeleted] = 0;

CREATE UNIQUE INDEX [UX_Companies_Name] ON [Companies] ([Name]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_Departments_CompanyId] ON [Departments] ([CompanyId]);

CREATE UNIQUE INDEX [IX_Departments_Name] ON [Departments] ([Name]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_Locations_CompanyId] ON [Locations] ([CompanyId]);

CREATE UNIQUE INDEX [IX_Locations_Name] ON [Locations] ([Name]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_Manufacturers_CompanyId] ON [Manufacturers] ([CompanyId]);

CREATE UNIQUE INDEX [IX_Manufacturers_Name] ON [Manufacturers] ([Name]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_OutboxMessages_ProcessedOnUtc] ON [OutboxMessages] ([ProcessedOnUtc]) WHERE [ProcessedOnUtc] IS NULL;

CREATE UNIQUE INDEX [UX_Permissions_Name] ON [Permissions] ([Name]);

CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);

CREATE UNIQUE INDEX [UX_Roles_Name] ON [Roles] ([Name]);

CREATE UNIQUE INDEX [IX_StatusLabels_Name] ON [StatusLabels] ([Name]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_Suppliers_CompanyId] ON [Suppliers] ([CompanyId]);

CREATE UNIQUE INDEX [IX_Suppliers_Name] ON [Suppliers] ([Name]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_Users_CompanyId] ON [Users] ([CompanyId]);

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);

CREATE UNIQUE INDEX [UX_Users_Email] ON [Users] ([Email]) WHERE [IsDeleted] = 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260707132304_AddMasterData', N'9.0.0');

ALTER TABLE [Assets] ADD [PeriodEnd] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';

ALTER TABLE [Assets] ADD [PeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [Assets] ADD PERIOD FOR SYSTEM_TIME ([PeriodStart], [PeriodEnd])

ALTER TABLE [Assets] ALTER COLUMN [PeriodStart] ADD HIDDEN

ALTER TABLE [Assets] ALTER COLUMN [PeriodEnd] ADD HIDDEN

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'ALTER TABLE [Assets] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + '].[AssetsHistory]))')


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260707133640_EnableTemporalAssets', N'9.0.0');

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [Accessories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [TotalQuantity] int NOT NULL,
    [PurchaseCost] decimal(18,2) NOT NULL,
    [PeriodEnd] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_Accessories] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([PeriodStart], [PeriodEnd])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[AccessoriesHistory]))');

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [Components] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [TotalQuantity] int NOT NULL,
    [PurchaseCost] decimal(18,2) NOT NULL,
    [PeriodEnd] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_Components] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([PeriodStart], [PeriodEnd])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[ComponentsHistory]))');

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [Consumables] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [TotalQuantity] int NOT NULL,
    [PurchaseCost] decimal(18,2) NOT NULL,
    [PeriodEnd] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_Consumables] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([PeriodStart], [PeriodEnd])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[ConsumablesHistory]))');

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [SoftwareLicenses] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [Name] nvarchar(255) NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [ManufacturerId] uniqueidentifier NULL,
    [TotalSeats] int NOT NULL,
    [PurchaseCost] decimal(18,2) NOT NULL,
    [ExpirationDate] datetime2 NULL,
    [Notes] nvarchar(max) NULL,
    [PeriodEnd] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_SoftwareLicenses] PRIMARY KEY ([Id]),
    PERIOD FOR SYSTEM_TIME([PeriodStart], [PeriodEnd])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[SoftwareLicensesHistory]))');

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [LicenseSeats] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
    [SoftwareLicenseId] uniqueidentifier NOT NULL,
    [AssignedUserId] uniqueidentifier NULL,
    [AssignedAssetId] uniqueidentifier NULL,
    [PeriodEnd] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [UpdatedBy] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAtUtc] datetime2 NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_LicenseSeats] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LicenseSeats_SoftwareLicenses_SoftwareLicenseId] FOREIGN KEY ([SoftwareLicenseId]) REFERENCES [SoftwareLicenses] ([Id]) ON DELETE CASCADE,
    PERIOD FOR SYSTEM_TIME([PeriodStart], [PeriodEnd])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[LicenseSeatsHistory]))');

CREATE UNIQUE INDEX [UX_Accessories_CompanyId_Name] ON [Accessories] ([CompanyId], [Name]) WHERE [IsDeleted] = 0;

CREATE UNIQUE INDEX [UX_Components_CompanyId_Name] ON [Components] ([CompanyId], [Name]) WHERE [IsDeleted] = 0;

CREATE UNIQUE INDEX [UX_Consumables_CompanyId_Name] ON [Consumables] ([CompanyId], [Name]) WHERE [IsDeleted] = 0;

CREATE INDEX [IX_LicenseSeats_SoftwareLicenseId] ON [LicenseSeats] ([SoftwareLicenseId]);

CREATE UNIQUE INDEX [UX_SoftwareLicenses_CompanyId_Name] ON [SoftwareLicenses] ([CompanyId], [Name]) WHERE [IsDeleted] = 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260707140456_AddInventoryAggregates', N'9.0.0');

COMMIT;
GO

