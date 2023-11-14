CREATE TABLE PropertyInformation (
    PropertyID INTEGER PRIMARY KEY,
    MapNumber TEXT,
    LotNumber TEXT,
    AccountNumber TEXT,
    CardNumber TEXT,
    CardTotal TEXT,
    LocationNumber TEXT,
    StreetName TEXT,
    LastUpdateDate DATE
);

CREATE TABLE OwnerInformation (
    OwnerID INTEGER PRIMARY KEY,
    CurrentOwner TEXT,
    SecondOwner TEXT,
    Street1 TEXT,
    Street2 TEXT,
    City TEXT,
    State TEXT,
    ZipCode TEXT
);
