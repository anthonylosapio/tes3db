CREATE TABLE `enchanting` (
    `id`                      VARCHAR(255),
    `effects`                 JSON,
    `enchant_type`            VARCHAR(255),
    `cost`                    INT,
    `max_charge`              INT,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
