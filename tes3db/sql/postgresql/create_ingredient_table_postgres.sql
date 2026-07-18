CREATE TABLE ingredient (
    flags         VARCHAR(255),
    id            VARCHAR(255),
    name          VARCHAR(255),
    mesh          VARCHAR(255),
    icon          VARCHAR(255),
    expansion     VARCHAR(255),
    weight        DOUBLE PRECISION,
    value         INTEGER,
    effects       JSONB,
    skills        JSONB,
    attributes    JSONB,
    PRIMARY KEY (id)
);