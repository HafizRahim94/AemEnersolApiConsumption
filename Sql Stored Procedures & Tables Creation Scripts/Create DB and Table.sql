CREATE DATABASE aemenersol

USE [aemenersol]
CREATE TABLE [dbo].[Platform](
	[id] [int] NULL,
	[uniqueName] [varchar](200) NULL,
	[latitude] [float] NULL,
	[longitude] [float] NULL,
	[CreatedAt] [datetime] NULL,
	[updatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[Well](
	[id] [int] NULL,
	[platformId] [int] NULL,
	[uniqueName] [varchar](200) NULL,
	[latitude] [float] NULL,
	[longitude] [float] NULL,
	[CreatedAt] [datetime] NULL,
	[updatedAt] [datetime] NULL
)