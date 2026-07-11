CREATE TABLE `faction` (
    `flags`                 VARCHAR(255),
    `id`                    VARCHAR(255),
    `name`                  VARCHAR(255),
    `rank_names`            JSON,
    `expansion`             VARCHAR(255),
    `reactions`             JSON,
    `favored_attribute1`    VARCHAR(255),
    `favored_attribute2`    VARCHAR(255),
    `requirements`          JSON,
    `favored_skills`        JSON,
    `data_flags`            VARCHAR(255),
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
