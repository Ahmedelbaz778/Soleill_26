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
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [QuestionnaireFields] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_QuestionnaireFields] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Doctors] (
    [Id] int NOT NULL IDENTITY,
    [Education] nvarchar(max) NOT NULL,
    [ExperienceYears] int NOT NULL,
    [NationalId] nvarchar(450) NOT NULL,
    [City] nvarchar(max) NOT NULL,
    [Street] nvarchar(max) NOT NULL,
    [Building] nvarchar(max) NULL,
    [ClinicPhone] nvarchar(max) NULL,
    [WorkingHours] nvarchar(max) NULL,
    [CertificateImage] nvarchar(max) NULL,
    [IsVerified] bit NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Doctors_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Parents] (
    [Id] int NOT NULL IDENTITY,
    [Relation] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Parents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Parents_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Games] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [FieldId] int NOT NULL,
    CONSTRAINT [PK_Games] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Games_QuestionnaireFields_FieldId] FOREIGN KEY ([FieldId]) REFERENCES [QuestionnaireFields] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Questions] (
    [Id] int NOT NULL IDENTITY,
    [Text] nvarchar(max) NOT NULL,
    [FieldId] int NOT NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Questions_QuestionnaireFields_FieldId] FOREIGN KEY ([FieldId]) REFERENCES [QuestionnaireFields] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Children] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [ParentId] int NOT NULL,
    CONSTRAINT [PK_Children] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Children_Parents_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Parents] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ChildProgresses] (
    [Id] int NOT NULL IDENTITY,
    [SocialPercentage] float NOT NULL,
    [CommunicationPercentage] float NOT NULL,
    [SkillsPercentage] float NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    [ChildId] int NOT NULL,
    CONSTRAINT [PK_ChildProgresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChildProgresses_Children_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [Children] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [EyeScanTests] (
    [Id] int NOT NULL IDENTITY,
    [ResultPercentage] float NOT NULL,
    [Date] datetime2 NOT NULL,
    [ChildId] int NOT NULL,
    CONSTRAINT [PK_EyeScanTests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EyeScanTests_Children_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [Children] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GameSessions] (
    [Id] int NOT NULL IDENTITY,
    [Date] datetime2 NOT NULL,
    [DurationInSeconds] int NOT NULL,
    [IsCompleted] bit NOT NULL,
    [ChildId] int NOT NULL,
    [GameId] int NOT NULL,
    CONSTRAINT [PK_GameSessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GameSessions_Children_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [Children] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GameSessions_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [QuestionnaireResults] (
    [Id] int NOT NULL IDENTITY,
    [Score] float NOT NULL,
    [Date] datetime2 NOT NULL,
    [ChildId] int NOT NULL,
    [FieldId] int NOT NULL,
    CONSTRAINT [PK_QuestionnaireResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_QuestionnaireResults_Children_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [Children] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_QuestionnaireResults_QuestionnaireFields_FieldId] FOREIGN KEY ([FieldId]) REFERENCES [QuestionnaireFields] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE UNIQUE INDEX [IX_ChildProgresses_ChildId] ON [ChildProgresses] ([ChildId]);

CREATE INDEX [IX_Children_ParentId] ON [Children] ([ParentId]);

CREATE UNIQUE INDEX [IX_Doctors_NationalId] ON [Doctors] ([NationalId]);

CREATE UNIQUE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);

CREATE INDEX [IX_EyeScanTests_ChildId] ON [EyeScanTests] ([ChildId]);

CREATE INDEX [IX_Games_FieldId] ON [Games] ([FieldId]);

CREATE INDEX [IX_GameSessions_ChildId] ON [GameSessions] ([ChildId]);

CREATE INDEX [IX_GameSessions_GameId] ON [GameSessions] ([GameId]);

CREATE UNIQUE INDEX [IX_Parents_UserId] ON [Parents] ([UserId]);

CREATE INDEX [IX_QuestionnaireResults_ChildId] ON [QuestionnaireResults] ([ChildId]);

CREATE INDEX [IX_QuestionnaireResults_FieldId] ON [QuestionnaireResults] ([FieldId]);

CREATE INDEX [IX_Questions_FieldId] ON [Questions] ([FieldId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260219142858_initalsoleil', N'9.0.0');

EXEC sp_rename N'[EyeScanTests].[ResultPercentage]', N'TdProbability', 'COLUMN';

ALTER TABLE [EyeScanTests] ADD [AsdProbability] float NOT NULL DEFAULT 0.0E0;

ALTER TABLE [EyeScanTests] ADD [Confidence] float NOT NULL DEFAULT 0.0E0;

ALTER TABLE [EyeScanTests] ADD [Decision] nvarchar(max) NOT NULL DEFAULT N'';

ALTER TABLE [EyeScanTests] ADD [PointsAnalyzed] int NOT NULL DEFAULT 0;

ALTER TABLE [EyeScanTests] ADD [Recommendation] nvarchar(max) NOT NULL DEFAULT N'';

ALTER TABLE [EyeScanTests] ADD [Result] nvarchar(max) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260506163719_updateeyescan', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260506180620_initalnew', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260507233407_initalnewupdate', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260508000433_intalnewww', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260508000752_Final', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260513022051_intalcreate', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260622142806_FinalizeNewAiIntegration', N'9.0.0');

COMMIT;
GO

