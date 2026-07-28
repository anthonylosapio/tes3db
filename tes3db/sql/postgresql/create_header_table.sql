CREATE TABLE header (
    flags           VARCHAR(255),
    version         VARCHAR(255),
    file_type       VARCHAR(255),
    author          VARCHAR(255),
    description     VARCHAR(255),
    num_objects     INT,
    masters         JSONB,
    expansion       VARCHAR(255)
);