-- ==========================================================
-- Smart Helpdesk Ticketing System - Database Schema
-- DBMS: SQL Server
-- ==========================================================

-- 1. Create Users Table
CREATE TABLE [Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [Role] NVARCHAR(50) NOT NULL, -- Admin, Agent, Customer
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL
);

-- 2. Create Tickets Table
CREATE TABLE [Tickets] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [Status] INT NOT NULL DEFAULT 0, -- 0: Open, 1: InProgress, 2: Resolved, 3: Closed
    [Priority] INT NOT NULL DEFAULT 1, -- 0: Low, 1: Medium, 2: High, 3: Urgent
    [CreatedByUserId] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Tickets_Users_CreatedBy] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users]([Id])
);

-- 3. Create TicketAssignments Table (For tracking assignment history)
CREATE TABLE [TicketAssignments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TicketId] INT NOT NULL,
    [AssignedToUserId] INT NOT NULL,
    [AssignedByUserId] INT NOT NULL,
    [AssignedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_Assignments_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [Tickets]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Assignments_Users_To] FOREIGN KEY ([AssignedToUserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Assignments_Users_By] FOREIGN KEY ([AssignedByUserId]) REFERENCES [Users]([Id])
);

-- 4. Create TicketComments Table
CREATE TABLE [TicketComments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TicketId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Comments_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [Tickets]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comments_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);

-- CREATE INDEXES for performance
CREATE INDEX [IX_Tickets_Status] ON [Tickets]([Status]);
CREATE INDEX [IX_Tickets_Priority] ON [Tickets]([Priority]);
CREATE INDEX [IX_Assignments_TicketId] ON [TicketAssignments]([TicketId]);
CREATE INDEX [IX_Comments_TicketId] ON [TicketComments]([TicketId]);
