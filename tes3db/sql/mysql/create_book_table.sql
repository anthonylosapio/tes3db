CREATE TABLE `book` (
    `id`               VARCHAR(255),
    `name`             VARCHAR(255),
    `mesh`             VARCHAR(255),
    `icon`             VARCHAR(255),
    `enchanting`       VARCHAR(255),
    `text`             TEXT,
    `expansion`        VARCHAR(255),
    `weight`           DOUBLE,
    `value`            INT,
    `book_type`        VARCHAR(255),
    `skill`            VARCHAR(255),
    `enchantment`      INT,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
