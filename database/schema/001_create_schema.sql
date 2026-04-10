-- Kudos App Database Schema
-- This file is for reference. EF Core migrations handle actual schema creation.

CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FullName TEXT NOT NULL,
    Role TEXT NOT NULL DEFAULT 'User',
    TotalPoints INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT
);

CREATE TABLE IF NOT EXISTS Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT,
    PointValue INTEGER NOT NULL,
    Icon TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Kudos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SenderId INTEGER NOT NULL,
    RecipientId INTEGER NOT NULL,
    CategoryId INTEGER NOT NULL,
    Message TEXT NOT NULL,
    PointsAwarded INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (SenderId) REFERENCES Users(Id),
    FOREIGN KEY (RecipientId) REFERENCES Users(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

CREATE TABLE IF NOT EXISTS Badges (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT NOT NULL,
    Icon TEXT,
    Criteria TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS UserBadges (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    BadgeId INTEGER NOT NULL,
    AwardedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (BadgeId) REFERENCES Badges(Id),
    UNIQUE(UserId, BadgeId)
);

CREATE TABLE IF NOT EXISTS Notifications (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Message TEXT NOT NULL,
    Type TEXT NOT NULL,
    IsRead INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS AuditLogs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    Action TEXT NOT NULL,
    EntityType TEXT NOT NULL,
    EntityId INTEGER,
    Details TEXT,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
