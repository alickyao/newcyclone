
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/27/2016 15:31:02
-- Generated from EDMX file: D:\project\git\newcyclone\NewCyclone\DataBase\SysModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [newcyclone];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Db_CatTree_inherits_Db_SysTree]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysTreeSet_Db_CatTree] DROP CONSTRAINT [FK_Db_CatTree_inherits_Db_SysTree];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_DocWeb_inherits_Db_SysDoc]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysDocSet_Db_DocWeb] DROP CONSTRAINT [FK_Db_DocWeb_inherits_Db_SysDoc];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_FileInfo_inherits_Db_FileSort]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysFileSet_Db_FileInfo] DROP CONSTRAINT [FK_Db_FileInfo_inherits_Db_FileSort];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_FileSort_inherits_Db_SysFileSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysFileSet_Db_FileSort] DROP CONSTRAINT [FK_Db_FileSort_inherits_Db_SysFileSet];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_ManagerUser_inherits_Db_SysUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysUserSet_Db_ManagerUser] DROP CONSTRAINT [FK_Db_ManagerUser_inherits_Db_SysUser];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_MemberUser_inherits_Db_SysUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysUserSet_Db_MemberUser] DROP CONSTRAINT [FK_Db_MemberUser_inherits_Db_SysUser];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_SysDocDb_DocCat]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysDocCatSet] DROP CONSTRAINT [FK_Db_SysDocDb_DocCat];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_SysDocDb_DocFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysDocFileSet] DROP CONSTRAINT [FK_Db_SysDocDb_DocFile];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_SysExceptionLog_inherits_Db_SysMsg]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysMsgSet_Db_SysExceptionLog] DROP CONSTRAINT [FK_Db_SysExceptionLog_inherits_Db_SysMsg];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_SysUserLog_inherits_Db_SysMsg]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysMsgSet_Db_SysUserLog] DROP CONSTRAINT [FK_Db_SysUserLog_inherits_Db_SysMsg];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_WebPage_inherits_Db_DocWeb]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysDocSet_Db_WebPage] DROP CONSTRAINT [FK_Db_WebPage_inherits_Db_DocWeb];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_WebRote_inherits_Db_DocWeb]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_SysDocSet_Db_WebRote] DROP CONSTRAINT [FK_Db_WebRote_inherits_Db_DocWeb];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Db_SysDocCatSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysDocCatSet];
GO
IF OBJECT_ID(N'[dbo].[Db_SysDocFileSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysDocFileSet];
GO
IF OBJECT_ID(N'[dbo].[Db_SysDocSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysDocSet];
GO
IF OBJECT_ID(N'[dbo].[Db_SysDocSet_Db_DocWeb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysDocSet_Db_DocWeb];
GO
IF OBJECT_ID(N'[dbo].[Db_SysDocSet_Db_WebPage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysDocSet_Db_WebPage];
GO
IF OBJECT_ID(N'[dbo].[Db_SysDocSet_Db_WebRote]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysDocSet_Db_WebRote];
GO
IF OBJECT_ID(N'[dbo].[Db_SysFileSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysFileSet];
GO
IF OBJECT_ID(N'[dbo].[Db_SysFileSet_Db_FileInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysFileSet_Db_FileInfo];
GO
IF OBJECT_ID(N'[dbo].[Db_SysFileSet_Db_FileSort]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysFileSet_Db_FileSort];
GO
IF OBJECT_ID(N'[dbo].[Db_SysMsgSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysMsgSet];
GO
IF OBJECT_ID(N'[dbo].[Db_SysMsgSet_Db_SysExceptionLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysMsgSet_Db_SysExceptionLog];
GO
IF OBJECT_ID(N'[dbo].[Db_SysMsgSet_Db_SysUserLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysMsgSet_Db_SysUserLog];
GO
IF OBJECT_ID(N'[dbo].[Db_SysTreeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysTreeSet];
GO
IF OBJECT_ID(N'[dbo].[Db_SysTreeSet_Db_CatTree]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysTreeSet_Db_CatTree];
GO
IF OBJECT_ID(N'[dbo].[Db_SysUserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysUserSet];
GO
IF OBJECT_ID(N'[dbo].[Db_SysUserSet_Db_ManagerUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysUserSet_Db_ManagerUser];
GO
IF OBJECT_ID(N'[dbo].[Db_SysUserSet_Db_MemberUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_SysUserSet_Db_MemberUser];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Db_SysUserSet'
CREATE TABLE [dbo].[Db_SysUserSet] (
    [loginName] nvarchar(50)  NOT NULL,
    [createdOn] datetime  NOT NULL,
    [isDeleted] bit  NOT NULL,
    [isDisabled] bit  NOT NULL,
    [lastLoginTime] datetime  NULL,
    [passWord] varchar(50)  NOT NULL,
    [role] varchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysMsgSet'
CREATE TABLE [dbo].[Db_SysMsgSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [createdOn] datetime  NOT NULL,
    [msgType] int  NOT NULL,
    [message] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Db_SysTreeSet'
CREATE TABLE [dbo].[Db_SysTreeSet] (
    [Id] varchar(50)  NOT NULL,
    [parentId] varchar(50)  NULL,
    [createdOn] datetime  NOT NULL,
    [isDeleted] bit  NOT NULL
);
GO

-- Creating table 'Db_SysFileSet'
CREATE TABLE [dbo].[Db_SysFileSet] (
    [Id] varchar(50)  NOT NULL,
    [createdOn] datetime  NOT NULL,
    [filePath] varchar(max)  NOT NULL,
    [fileName] varchar(max)  NOT NULL,
    [createdBy] varchar(max)  NOT NULL
);
GO

-- Creating table 'Db_SysDocSet'
CREATE TABLE [dbo].[Db_SysDocSet] (
    [Id] nvarchar(50)  NOT NULL,
    [createdOn] datetime  NOT NULL,
    [createdBy] nvarchar(50)  NOT NULL,
    [modifiedOn] datetime  NOT NULL,
    [modifiedBy] nvarchar(50)  NOT NULL,
    [isDeleted] bit  NOT NULL,
    [caption] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Db_SysDocCatSet'
CREATE TABLE [dbo].[Db_SysDocCatSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Db_SysDocId] nvarchar(50)  NOT NULL,
    [Db_CatTreeId] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysDocFileSet'
CREATE TABLE [dbo].[Db_SysDocFileSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Db_SysDocId] nvarchar(50)  NOT NULL,
    [Db_SysFileId] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysUserSet_Db_ManagerUser'
CREATE TABLE [dbo].[Db_SysUserSet_Db_ManagerUser] (
    [fullName] nvarchar(50)  NOT NULL,
    [mobilePhone] nvarchar(50)  NULL,
    [jobTitle] nvarchar(50)  NULL,
    [loginName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysUserSet_Db_MemberUser'
CREATE TABLE [dbo].[Db_SysUserSet_Db_MemberUser] (
    [nickName] nvarchar(50)  NULL,
    [loginName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysMsgSet_Db_SysExceptionLog'
CREATE TABLE [dbo].[Db_SysMsgSet_Db_SysExceptionLog] (
    [errorCode] int  NOT NULL,
    [condtion] varchar(max)  NULL,
    [source] varchar(max)  NOT NULL,
    [stackTrace] varchar(max)  NOT NULL,
    [targetSite] varchar(max)  NOT NULL,
    [Id] bigint  NOT NULL
);
GO

-- Creating table 'Db_SysMsgSet_Db_SysUserLog'
CREATE TABLE [dbo].[Db_SysMsgSet_Db_SysUserLog] (
    [Db_SysUser_loginName] nvarchar(50)  NOT NULL,
    [logType] int  NOT NULL,
    [fkId] nvarchar(50)  NULL,
    [ip] varchar(50)  NULL,
    [device] varchar(max)  NULL,
    [Id] bigint  NOT NULL
);
GO

-- Creating table 'Db_SysTreeSet_Db_CatTree'
CREATE TABLE [dbo].[Db_SysTreeSet_Db_CatTree] (
    [name] nvarchar(max)  NOT NULL,
    [fun] varchar(50)  NULL,
    [sort] int  NULL,
    [alias] nvarchar(50)  NULL,
    [Id] varchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysDocSet_Db_DocWeb'
CREATE TABLE [dbo].[Db_SysDocSet_Db_DocWeb] (
    [fun] nvarchar(max)  NOT NULL,
    [describe] nvarchar(max)  NULL,
    [showTime] datetime  NOT NULL,
    [alias] nvarchar(50)  NULL,
    [Id] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysDocSet_Db_WebPage'
CREATE TABLE [dbo].[Db_SysDocSet_Db_WebPage] (
    [seoTitle] nvarchar(max)  NULL,
    [seoKeyWords] nvarchar(max)  NULL,
    [content] nvarchar(max)  NULL,
    [Id] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysDocSet_Db_WebRote'
CREATE TABLE [dbo].[Db_SysDocSet_Db_WebRote] (
    [imgWidth] int  NOT NULL,
    [imgHeight] int  NOT NULL,
    [waitSecond] int  NOT NULL,
    [Id] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysFileSet_Db_FileSort'
CREATE TABLE [dbo].[Db_SysFileSet_Db_FileSort] (
    [sort] int  NOT NULL,
    [Id] varchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysFileSet_Db_FileInfo'
CREATE TABLE [dbo].[Db_SysFileSet_Db_FileInfo] (
    [title] nvarchar(max)  NULL,
    [describe] nvarchar(max)  NULL,
    [link] varchar(max)  NULL,
    [Id] varchar(50)  NOT NULL
);
GO

-- Creating table 'Db_SysMsgSet_Db_SysNotice'
CREATE TABLE [dbo].[Db_SysMsgSet_Db_SysNotice] (
    [alert] bit  NOT NULL,
    [title] nvarchar(max)  NOT NULL,
    [isRead] bit  NOT NULL,
    [readTime] datetime  NULL,
    [readUser] nvarchar(50)  NULL,
    [linkUrl] nvarchar(max)  NULL,
    [Id] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [loginName] in table 'Db_SysUserSet'
ALTER TABLE [dbo].[Db_SysUserSet]
ADD CONSTRAINT [PK_Db_SysUserSet]
    PRIMARY KEY CLUSTERED ([loginName] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysMsgSet'
ALTER TABLE [dbo].[Db_SysMsgSet]
ADD CONSTRAINT [PK_Db_SysMsgSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysTreeSet'
ALTER TABLE [dbo].[Db_SysTreeSet]
ADD CONSTRAINT [PK_Db_SysTreeSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysFileSet'
ALTER TABLE [dbo].[Db_SysFileSet]
ADD CONSTRAINT [PK_Db_SysFileSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysDocSet'
ALTER TABLE [dbo].[Db_SysDocSet]
ADD CONSTRAINT [PK_Db_SysDocSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysDocCatSet'
ALTER TABLE [dbo].[Db_SysDocCatSet]
ADD CONSTRAINT [PK_Db_SysDocCatSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysDocFileSet'
ALTER TABLE [dbo].[Db_SysDocFileSet]
ADD CONSTRAINT [PK_Db_SysDocFileSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [loginName] in table 'Db_SysUserSet_Db_ManagerUser'
ALTER TABLE [dbo].[Db_SysUserSet_Db_ManagerUser]
ADD CONSTRAINT [PK_Db_SysUserSet_Db_ManagerUser]
    PRIMARY KEY CLUSTERED ([loginName] ASC);
GO

-- Creating primary key on [loginName] in table 'Db_SysUserSet_Db_MemberUser'
ALTER TABLE [dbo].[Db_SysUserSet_Db_MemberUser]
ADD CONSTRAINT [PK_Db_SysUserSet_Db_MemberUser]
    PRIMARY KEY CLUSTERED ([loginName] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysMsgSet_Db_SysExceptionLog'
ALTER TABLE [dbo].[Db_SysMsgSet_Db_SysExceptionLog]
ADD CONSTRAINT [PK_Db_SysMsgSet_Db_SysExceptionLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysMsgSet_Db_SysUserLog'
ALTER TABLE [dbo].[Db_SysMsgSet_Db_SysUserLog]
ADD CONSTRAINT [PK_Db_SysMsgSet_Db_SysUserLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysTreeSet_Db_CatTree'
ALTER TABLE [dbo].[Db_SysTreeSet_Db_CatTree]
ADD CONSTRAINT [PK_Db_SysTreeSet_Db_CatTree]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysDocSet_Db_DocWeb'
ALTER TABLE [dbo].[Db_SysDocSet_Db_DocWeb]
ADD CONSTRAINT [PK_Db_SysDocSet_Db_DocWeb]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysDocSet_Db_WebPage'
ALTER TABLE [dbo].[Db_SysDocSet_Db_WebPage]
ADD CONSTRAINT [PK_Db_SysDocSet_Db_WebPage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysDocSet_Db_WebRote'
ALTER TABLE [dbo].[Db_SysDocSet_Db_WebRote]
ADD CONSTRAINT [PK_Db_SysDocSet_Db_WebRote]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysFileSet_Db_FileSort'
ALTER TABLE [dbo].[Db_SysFileSet_Db_FileSort]
ADD CONSTRAINT [PK_Db_SysFileSet_Db_FileSort]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysFileSet_Db_FileInfo'
ALTER TABLE [dbo].[Db_SysFileSet_Db_FileInfo]
ADD CONSTRAINT [PK_Db_SysFileSet_Db_FileInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_SysMsgSet_Db_SysNotice'
ALTER TABLE [dbo].[Db_SysMsgSet_Db_SysNotice]
ADD CONSTRAINT [PK_Db_SysMsgSet_Db_SysNotice]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Db_SysDocId] in table 'Db_SysDocCatSet'
ALTER TABLE [dbo].[Db_SysDocCatSet]
ADD CONSTRAINT [FK_Db_SysDocDb_DocCat]
    FOREIGN KEY ([Db_SysDocId])
    REFERENCES [dbo].[Db_SysDocSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Db_SysDocDb_DocCat'
CREATE INDEX [IX_FK_Db_SysDocDb_DocCat]
ON [dbo].[Db_SysDocCatSet]
    ([Db_SysDocId]);
GO

-- Creating foreign key on [Db_SysDocId] in table 'Db_SysDocFileSet'
ALTER TABLE [dbo].[Db_SysDocFileSet]
ADD CONSTRAINT [FK_Db_SysDocDb_DocFile]
    FOREIGN KEY ([Db_SysDocId])
    REFERENCES [dbo].[Db_SysDocSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Db_SysDocDb_DocFile'
CREATE INDEX [IX_FK_Db_SysDocDb_DocFile]
ON [dbo].[Db_SysDocFileSet]
    ([Db_SysDocId]);
GO

-- Creating foreign key on [loginName] in table 'Db_SysUserSet_Db_ManagerUser'
ALTER TABLE [dbo].[Db_SysUserSet_Db_ManagerUser]
ADD CONSTRAINT [FK_Db_ManagerUser_inherits_Db_SysUser]
    FOREIGN KEY ([loginName])
    REFERENCES [dbo].[Db_SysUserSet]
        ([loginName])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [loginName] in table 'Db_SysUserSet_Db_MemberUser'
ALTER TABLE [dbo].[Db_SysUserSet_Db_MemberUser]
ADD CONSTRAINT [FK_Db_MemberUser_inherits_Db_SysUser]
    FOREIGN KEY ([loginName])
    REFERENCES [dbo].[Db_SysUserSet]
        ([loginName])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysMsgSet_Db_SysExceptionLog'
ALTER TABLE [dbo].[Db_SysMsgSet_Db_SysExceptionLog]
ADD CONSTRAINT [FK_Db_SysExceptionLog_inherits_Db_SysMsg]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysMsgSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysMsgSet_Db_SysUserLog'
ALTER TABLE [dbo].[Db_SysMsgSet_Db_SysUserLog]
ADD CONSTRAINT [FK_Db_SysUserLog_inherits_Db_SysMsg]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysMsgSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysTreeSet_Db_CatTree'
ALTER TABLE [dbo].[Db_SysTreeSet_Db_CatTree]
ADD CONSTRAINT [FK_Db_CatTree_inherits_Db_SysTree]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysTreeSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysDocSet_Db_DocWeb'
ALTER TABLE [dbo].[Db_SysDocSet_Db_DocWeb]
ADD CONSTRAINT [FK_Db_DocWeb_inherits_Db_SysDoc]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysDocSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysDocSet_Db_WebPage'
ALTER TABLE [dbo].[Db_SysDocSet_Db_WebPage]
ADD CONSTRAINT [FK_Db_WebPage_inherits_Db_DocWeb]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysDocSet_Db_DocWeb]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysDocSet_Db_WebRote'
ALTER TABLE [dbo].[Db_SysDocSet_Db_WebRote]
ADD CONSTRAINT [FK_Db_WebRote_inherits_Db_DocWeb]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysDocSet_Db_DocWeb]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysFileSet_Db_FileSort'
ALTER TABLE [dbo].[Db_SysFileSet_Db_FileSort]
ADD CONSTRAINT [FK_Db_FileSort_inherits_Db_SysFileSet]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysFileSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysFileSet_Db_FileInfo'
ALTER TABLE [dbo].[Db_SysFileSet_Db_FileInfo]
ADD CONSTRAINT [FK_Db_FileInfo_inherits_Db_FileSort]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysFileSet_Db_FileSort]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_SysMsgSet_Db_SysNotice'
ALTER TABLE [dbo].[Db_SysMsgSet_Db_SysNotice]
ADD CONSTRAINT [FK_Db_SysNotice_inherits_Db_SysMsg]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_SysMsgSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------