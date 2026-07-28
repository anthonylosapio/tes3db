CREATE TABLE `header` (
    `flags`            VARCHAR(255),
    `version`          VARCHAR(255),
    `file_type`        VARCHAR(255),
    `author`           VARCHAR(255),
    `description`      VARCHAR(255),
    `num_objects`      INT,
    `masters`          JSON,
    `expansion`        VARCHAR(255)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
