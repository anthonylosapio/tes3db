CREATE TABLE enchanting (
    id             VARCHAR(255),
    effects        JSONB,
    enchant_type   VARCHAR(255),
    cost           INTEGER,
    max_charge     INTEGER,
    PRIMARY KEY (id)
);