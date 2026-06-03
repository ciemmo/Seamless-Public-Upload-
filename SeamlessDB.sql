DROP DATABASE IF EXISTS SeamlessDB;
CREATE DATABASE SeamlessDB;
USE SeamlessDB;

-- Closet
create table Closet(
    userID int primary key,
    username varchar(64),
    passwordHash varbinary(64),
    email varchar(64),
    role varchar(64),
    is2FAEnabled bool,
    securityKey2FA varbinary(64),
    closetID int,
    profileID int,
    foreign key (closetID) references VirtualCloset(closetID), -- 1-1
    foreign key (profileID) references Profile(profileID) -- 1-1
);

-- Virtual Closet
create table VirtualCloset(
    closetID int primary key,
    clothingItems varchar(64),
    clothingObjects varchar(64),
    notificationsID int,
    foreign key (notificationsID) references TrashCan (notificationsID) -- 1-1
);

-- Clothing Item
create table ClothingItem(
    itemID int primary key,
    itemName varchar(32),
    photoURL varchar(128),
    tags  varchar(64),
    type varchar(16),
    personalNotes varchar(128),
    lastWornDate varchar(12),
    wornStatus varchar(16),
    closetID int,
    outfitID int,
    tagID int,
    foreign key (closetID) references VirtualCloset (closetID), -- 1-many
    foreign key (outfitID) references Outfits (outfitID), -- many-many
    foreign key (tagID) references Tags (tagID) -- many-many 
);

-- Outfits
create table Outfits(
    outfitID int primary key,
    outfitName varchar(64),
    tags varchar(64),
    photoURL varchar(128),
    personalNotes varchar(128),
    clothingItems varchar(64),
    closetID int,
    tagID int,
    foreign key (closetID) references VirtualCloset(closetID), -- 1-many
    foreign key (tagID) references Tags (tagID) -- many-many
);

-- Suggested Outfit
create table SuggestedOutfit(
    outfitID int,
    outfitName varchar(32),
    tags varchar(64),
    photoURL varchar(128),
    personalNotes varchar(128),
    status varchar(32),
    closetID int,
    tagID int,
    profileID int,
    foreign key (closetID) references VirtualCloset (closetID), -- 1-many
    foreign key (tagID) references Tag (tagID), -- many-many
    foreign key (profileID) references Profile (profileID) -- 1-many
);

-- User Profile
create table Profile(
    profileID int primary key,
    displayName varchar(32),
    profileImageURL varchar(128),
    PrivateAccount bool,
    postID int,
    CollectionofPost int,
    CollectionofFollows int,
    CollectionofFollowers int,
    foreign key (postID) references Post (postID) -- 1-many
);

-- Collections
create table Collections(
    collectionID int,
    collectionName varchar(64),
    collectionTags varchar(64),
    collectionDescription varchar(128)
);

-- Post
create table Post(
    postID int,
    outfitID int,
    profileID int,
    timestamp varchar(32),
    pictureURL varchar(128),
    caption varchar(64),
    foreign key (profileID) references Profile (profileID), -- many-1
    foreign key (outfitID) references Outfit (outfitID) -- 1-1
);

-- Like
create table Likes(
    likeID int,
    profileID int,
    postID int,
    timestamp varchar(32),
    foreign key (profileID) references Profile (profileID), -- many-1
    foreign key (postID) references Post (postID) -- many-1
);

-- Tag
create table Tag(
    tagID int,
    tagName varchar(64),
    tagCategory varchar(128)
);

-- Weather
create table Weather(
    temperature varchar(32),
    weathercond varchar(16),
    location varchar(64),
    tagID int,
    foreign key (tagID) references Tag (tagID) -- many-many
);

-- Trash Item
create table TrashItem(
    deletionDate varchar(32),
    referenceToDeletedItem varchar(16)
);

-- Trash Can
create table TrashCan(
    notificationsID int primary key,
    message varchar(64),
    isRead bool,
    timestamp varchar(32),
    profileID int,
    foreign key (profileID) references Profile (profileID) -- many-1
);