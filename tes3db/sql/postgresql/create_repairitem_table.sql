CREATE TABLE repairitem (
    flags           VARCHAR(255),
    id              VARCHAR(255) PRIMARY KEY,
    name            VARCHAR(255),
    mesh            VARCHAR(255),
    icon            VARCHAR(255),
    expansion       VARCHAR(255),
    weight          DOUBLE PRECISION,
    quality         DOUBLE PRECISION,
    value           INT,
    uses            INT
);