IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'seasons'))
BEGIN
	CREATE TABLE [dbo].[seasons](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Year] [nvarchar](7) NOT NULL,
	 CONSTRAINT [PK_Seasons] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'leagues'))
BEGIN
	CREATE TABLE [dbo].[leagues](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[LeagueId] [int] NOT NULL,
		[SeasonId] [int] NOT NULL,
		[Name] [nvarchar](max) NULL,
	 CONSTRAINT [PK_Leagues] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	 CONSTRAINT [AK_Leagues_LeagueId] UNIQUE NONCLUSTERED 
	(
		[LeagueId] ASC,
		[SeasonId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[leagues]  WITH CHECK ADD  CONSTRAINT [FK_Leagues_Seasons_SeasonId] FOREIGN KEY([SeasonId])
	REFERENCES [dbo].[seasons] ([Id])

	ALTER TABLE [dbo].[leagues] CHECK CONSTRAINT [FK_Leagues_Seasons_SeasonId]
END
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'players'))
BEGIN
	CREATE TABLE [dbo].[players](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[LeagueId] [int] NOT NULL,
		[EntryId] [int] NOT NULL,
		[PlayerName] [nvarchar](max) NULL,
		[TeamName] [nvarchar](max) NULL,
	 CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'teams'))
BEGIN
	CREATE TABLE [dbo].[teams](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[SeasonId] [int] NOT NULL,
		[TeamId] [int] NOT NULL,
		[Code] [int] NOT NULL,
		[Name] [nvarchar](max) NULL,
		[ShortName] [nvarchar](3) NULL,
	 CONSTRAINT [PK_Teams] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[teams]  WITH CHECK ADD  CONSTRAINT [FK_Teams_Seasons_SeasonId] FOREIGN KEY([SeasonId])
	REFERENCES [dbo].[seasons] ([Id])

	ALTER TABLE [dbo].[teams] CHECK CONSTRAINT [FK_Teams_Seasons_SeasonId]
END
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'elements'))
BEGIN
	CREATE TABLE [dbo].[elements](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Code] [int] NOT NULL,
		[TeamId] [int] NOT NULL,
		[FirstName] [nvarchar](max) NULL,
		[SecondName] [nvarchar](max) NULL,
		[WebName] [nvarchar](max) NULL,
		[ElementId] [int] NOT NULL,
		[ElementTeamId] [int] NOT NULL,
		[ElementType] [int] NOT NULL,
	 CONSTRAINT [PK_Elements] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[elements] ADD  DEFAULT ((0)) FOR [ElementId]

	ALTER TABLE [dbo].[elements] ADD  DEFAULT ((0)) FOR [ElementTeamId]

	ALTER TABLE [dbo].[elements] ADD  DEFAULT ((0)) FOR [ElementType]

	ALTER TABLE [dbo].[elements]  WITH CHECK ADD  CONSTRAINT [FK_Elements_Teams_TeamId] FOREIGN KEY([TeamId])
	REFERENCES [dbo].[teams] ([Id])

	ALTER TABLE [dbo].[elements] CHECK CONSTRAINT [FK_Elements_Teams_TeamId]
END
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'elements_stats'))
BEGIN
	CREATE TABLE [dbo].[element_stats](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[ElementId] [int] NOT NULL,
		[Gameweek] [int] NOT NULL,
		[Minutes] [int] NOT NULL,
		[GoalsScored] [int] NOT NULL,
		[Assists] [int] NOT NULL,
		[CleanSheets] [int] NOT NULL,
		[GoalsConceded] [int] NOT NULL,
		[OwnGoals] [int] NOT NULL,
		[PenaltiesSaved] [int] NOT NULL,
		[PenaltiesMissed] [int] NOT NULL,
		[YellowCards] [int] NOT NULL,
		[RedCards] [int] NOT NULL,
		[Saves] [int] NOT NULL,
		[Bonus] [int] NOT NULL,
		[Bps] [int] NOT NULL,
		[Influence] [nvarchar](max) NULL,
		[Creativity] [nvarchar](max) NULL,
		[Threat] [nvarchar](max) NULL,
		[IctIndex] [nvarchar](max) NULL,
		[TotalPoints] [int] NOT NULL,
		[InDreamteam] [bit] NOT NULL,
		[ApiElementId] [int] NOT NULL,
	 CONSTRAINT [PK_ElementStats] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[Elements_Stats] ADD  DEFAULT ((0)) FOR [ApiElementId]

	ALTER TABLE [dbo].[Elements_Stats]  WITH CHECK ADD  CONSTRAINT [FK_ElementStats_Elements_ElementId] FOREIGN KEY([ElementId])
	REFERENCES [dbo].[Elements] ([Id])

	ALTER TABLE [dbo].[Elements_Stats] CHECK CONSTRAINT [FK_ElementStats_Elements_ElementId]

END
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'picks'))
BEGIN
	CREATE TABLE [dbo].[picks](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[PlayerId] [int] NOT NULL,
		[Gameweek] [int] NOT NULL,
		[ElementId] [int] NOT NULL,
		[Multiplier] [int] NOT NULL,
		[Position] [int] NOT NULL,
	 CONSTRAINT [PK_Picks] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[picks] ADD  DEFAULT ((0)) FOR [Position]

	ALTER TABLE [dbo].[picks]  WITH CHECK ADD  CONSTRAINT [FK_Picks_Players_PlayerId] FOREIGN KEY([PlayerId])
	REFERENCES [dbo].[players] ([Id])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[picks] CHECK CONSTRAINT [FK_Picks_Players_PlayerId]
END
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'points'))
BEGIN
	CREATE TABLE [dbo].[points](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[PlayerId] [int] NOT NULL,
		[Gameweek] [int] NOT NULL,
		[GameweekPoints] [int] NOT NULL,
		[GameweekPointsOnBench] [int] NOT NULL,
		[Total] [int] NOT NULL,
	 CONSTRAINT [PK_Points] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Points]  WITH CHECK ADD  CONSTRAINT [FK_Points_Players_PlayerId] FOREIGN KEY([PlayerId])
	REFERENCES [dbo].[Players] ([Id])

	ALTER TABLE [dbo].[Points] CHECK CONSTRAINT [FK_Points_Players_PlayerId]
END
GO