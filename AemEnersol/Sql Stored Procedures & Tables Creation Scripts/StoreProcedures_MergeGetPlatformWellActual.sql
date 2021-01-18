SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hafizuddin
-- Create date: 2021-01-16
-- Description:	To Insert Platform and Well data.
-- =============================================
CREATE PROCEDURE Merge_GetPlatformWellActual 
	(
	@strInsType nvarchar,
	@intPlatformID int,
	@intID int,
	@strUniqueName VARCHAR(50),
	@Fltlatitude Float,
	@Fltlongitude Float,
	@DtcreatedAt DateTime,
	@DtupdatedAt DateTime
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	CREATE TABLE #Platform(
		[id] [int] NULL,
		[uniqueName] [varchar](200) NULL,
		[latitude] [float] NULL,
		[longitude] [float] NULL,
		[CreatedAt] [datetime] NULL,
		[updatedAt] [datetime] NULL
	)

	CREATE TABLE #Well(
		[id] [int] NULL,
		[platformId] [int] NULL,
		[uniqueName] [varchar](200) NULL,
		[latitude] [float] NULL,
		[longitude] [float] NULL,
		[CreatedAt] [datetime] NULL,
		[updatedAt] [datetime] NULL
	)
	
	
	IF(@strInsType = 'P')
	BEGIN
		INSERT INTO #Platform VALUES (@intID,@strUniqueName,@Fltlatitude,@Fltlongitude,@DtcreatedAt,@DtupdatedAt)
		MERGE [Platform] AS t
		USING #Platform AS s
		ON (t.id = s.id) 
		--When records are matched, update the records if there is any change
		WHEN MATCHED AND 
			t.uniqueName <> s.uniqueName OR t.latitude <> s.latitude OR
			t.longitude <> s.longitude
		THEN UPDATE SET t.uniqueName = s.uniqueName, t.latitude = s.latitude,t.longitude = s.longitude,t.CreatedAt = s.CreatedAt,t.updatedAt = s.updatedAt 
		WHEN NOT MATCHED BY TARGET 
		THEN INSERT VALUES (s.id, s.uniqueName, s.latitude,s.longitude,s.CreatedAt,s.updatedAt);
	END

	IF(@strInsType = 'W')
	BEGIN
		INSERT INTO #Well VALUES (@intID,@intPlatformID,@strUniqueName,@Fltlatitude,@Fltlongitude,@DtcreatedAt,@DtupdatedAt)
		MERGE [Well] AS t
		USING #Well AS s
		ON (t.platformId = s.platformId AND t.id=s.id) 
		WHEN MATCHED AND 
			t.uniqueName <> s.uniqueName OR t.latitude <> s.latitude OR
			t.longitude <> s.longitude
		THEN UPDATE SET t.uniqueName = s.uniqueName, t.latitude = s.latitude,t.longitude = s.longitude,t.CreatedAt = s.CreatedAt,t.updatedAt = s.updatedAt 
		WHEN NOT MATCHED BY TARGET 
		THEN INSERT VALUES (s.id, s.platformid,s.uniqueName, s.latitude,s.longitude,s.CreatedAt,s.updatedAt);
	END
	
	--select * FROM aemenersol..[Platform]
	--select * FROM aemenersol..[Well]
END
GO
