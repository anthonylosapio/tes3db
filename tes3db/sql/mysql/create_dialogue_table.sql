CREATE TABLE `dialogue` (
    `id`                      VARCHAR(255),
    `prev_id`                 VARCHAR(255),
    `next_id`                 VARCHAR(255),
    `speaker_id`              VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin,
    `speaker_race`            VARCHAR(255),
    `speaker_class`           VARCHAR(255),
    `speaker_faction`         VARCHAR(255),
    `speaker_cell`            VARCHAR(255),
    `player_faction`          VARCHAR(255),
    `text`                    TEXT,
    `expansion`               VARCHAR(255),
    `dialogue_type`           VARCHAR(255),
    `disposition`             INT,
    `speaker_rank`            INT,
    `speaker_sex`             VARCHAR(255),
    `player_rank`             INT,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
