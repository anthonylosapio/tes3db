CREATE TABLE lockpick (
    flags        VARCHAR(255),
    id           VARCHAR(255),
    name         VARCHAR(255),
    mesh         VARCHAR(255),
    icon         VARCHAR(255),
    expansion    VARCHAR(255),
    weight       DOUBLE PRECISION,
    quality      DOUBLE PRECISION,
    value        INTEGER,
    uses         INTEGER,
    PRIMARY KEY (id)
);