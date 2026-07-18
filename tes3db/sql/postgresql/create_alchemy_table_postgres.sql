CREATE TABLE alchemy (
    flags        VARCHAR(255),
    id           VARCHAR(255),
    name         VARCHAR(255),
    mesh         VARCHAR(255),
    icon         VARCHAR(255),
    expansion    VARCHAR(255),
    effects      JSONB,
    weight       DOUBLE PRECISION,
    value        INTEGER,
    data_flags   VARCHAR(255),
    PRIMARY KEY (id)
);