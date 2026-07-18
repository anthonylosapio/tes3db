CREATE TABLE birthsign (
    flags          VARCHAR(255),
    id             VARCHAR(255),
    name           VARCHAR(255),
    texture        VARCHAR(255),
    description    TEXT,
    spells         JSONB,
    PRIMARY KEY (id)
);