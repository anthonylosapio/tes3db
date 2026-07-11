CREATE TABLE `lockpick` (
    `flags`            VARCHAR(255),
    `id`               VARCHAR(255),
    `name`             VARCHAR(255),
    `mesh`             VARCHAR(255),
    `icon`             VARCHAR(255),
    `expansion`        VARCHAR(255),
    `weight`           DOUBLE,
    `quality`          DOUBLE,
    `value`            INT,
    `uses`            INT,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
