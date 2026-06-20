CREATE TABLE `weapon` (
    `flags`            VARCHAR(255),
    `id`               VARCHAR(255),
    `name`             VARCHAR(255),
    `mesh`             VARCHAR(255),
    `icon`             VARCHAR(255),
    `enchanting`       VARCHAR(255),
    `expansion`        VARCHAR(255),
    `armor_type`       VARCHAR(255),
    `weight`           DOUBLE,
    `value`            INT,
    `health`           INT,
    `enchantment`      INT,
    `armor_rating`     INT,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
