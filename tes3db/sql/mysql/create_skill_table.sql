CREATE TABLE `skill` (
    `flags`                VARCHAR(255),
    `id`                   VARCHAR(255),
    `description`          TEXT,
    `expansion`            VARCHAR(255),
    `governing_attribute`  VARCHAR(255),
    `specialization`       VARCHAR(255),
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
