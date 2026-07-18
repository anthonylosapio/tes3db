CREATE TABLE apparatus (
    flags            VARCHAR(255),
    id               VARCHAR(255),
    name             VARCHAR(255),
    mesh             VARCHAR(255),
    icon             VARCHAR(255),
    expansion        VARCHAR(255),
    apparatus_type   VARCHAR(255),
    quality          DOUBLE PRECISION,
    weight           DOUBLE PRECISION,
    value            INTEGER,
    PRIMARY KEY (id)
);