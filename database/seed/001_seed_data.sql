-- Kudos App Seed Data
-- This file is for reference. EF Core DataSeeder handles actual seeding.

-- Default Categories
INSERT INTO Categories (Name, Description, PointValue, Icon) VALUES
('Teamwork', 'Great collaboration and team spirit', 10, '🤝'),
('Innovation', 'Creative solutions and new ideas', 25, '💡'),
('Leadership', 'Guiding and inspiring others', 20, '⭐'),
('Excellence', 'Outstanding quality of work', 30, '🏆'),
('Helpfulness', 'Going above and beyond to help', 15, '🙌');

-- Default Admin User (password: Admin123!)
-- PasswordHash is a placeholder — actual hash generated at runtime
INSERT INTO Users (Email, PasswordHash, FullName, Role) VALUES
('admin@kudosapp.com', 'HASHED_AT_RUNTIME', 'System Admin', 'Admin');

-- Default Badges
INSERT INTO Badges (Name, Description, Icon, Criteria) VALUES
('First Kudos', 'Gave your first kudos', '🎉', 'kudos_given >= 1'),
('Team Player', 'Gave 10 kudos', '🤝', 'kudos_given >= 10'),
('Star Receiver', 'Received 10 kudos', '⭐', 'kudos_received >= 10'),
('Point Master', 'Accumulated 100 points', '💯', 'total_points >= 100'),
('Top Contributor', 'Accumulated 500 points', '🏅', 'total_points >= 500');
