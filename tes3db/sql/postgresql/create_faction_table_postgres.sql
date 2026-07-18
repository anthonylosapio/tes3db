CREATE TABLE faction (
    flags                 VARCHAR(255),
    id                    VARCHAR(255),
    name                  VARCHAR(255),
    rank_names            JSONB,
    expansion             VARCHAR(255),
    reactions             JSONB,
    favored_attribute1    VARCHAR(255),
    favored_attribute2    VARCHAR(255),
    requirements          JSONB,
    favored_skills        JSONB,
    data_flags            VARCHAR(255),
    PRIMARY KEY (id)
);