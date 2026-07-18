CREATE TABLE armor (
    flags           VARCHAR(255),
    id              VARCHAR(255),
    name            VARCHAR(255),
    mesh            VARCHAR(255),
    icon            VARCHAR(255),
    enchanting      VARCHAR(255),
    expansion       VARCHAR(255),
    armor_type      VARCHAR(255),
    weight          DOUBLE PRECISION,
    value           INTEGER,
    health          INTEGER,
    enchantment     INTEGER,
    armor_rating    INTEGER,
    PRIMARY KEY (id)
);