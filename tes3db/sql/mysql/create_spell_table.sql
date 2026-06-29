CREATE TABLE `spell` (
    `flags`            VARCHAR(255),
    `id`               VARCHAR(255),
    `name`             VARCHAR(255),
    `effects`          JSON,
    `expansion`        VARCHAR(255),
    `spell_type`       VARCHAR(255),
    `cost`             INT,
    `data_flags`       VARCHAR(255),
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
