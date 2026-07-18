CREATE TABLE book (
    id             VARCHAR(255),
    name           VARCHAR(255),
    mesh           VARCHAR(255),
    icon           VARCHAR(255),
    enchanting     VARCHAR(255),
    text           TEXT,
    expansion      VARCHAR(255),
    weight         DOUBLE PRECISION,
    value          INTEGER,
    book_type      VARCHAR(255),
    skill          VARCHAR(255),
    enchantment    INTEGER,
    PRIMARY KEY (id)
);