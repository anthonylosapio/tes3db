CREATE TABLE `dialogueinfo` (
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
    `dialogue_topic`          VARCHAR(255),
    `dialogue_id`             INT,
    `dialogue_type`           VARCHAR(255),
    `disposition`             INT,
    `speaker_rank`            INT,
    `speaker_sex`             VARCHAR(255),
    `player_rank`             INT,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `idx_dialogueinfo_next_id` ON `dialogueinfo` (`next_id`, `dialogue_topic`);
CREATE INDEX `idx_dialogueinfo_topic_id` ON `dialogueinfo` (`dialogue_topic`, `id`);
CREATE INDEX `idx_dialogueinfo_speaker_id` ON `dialogueinfo` (`speaker_id`);
CREATE INDEX `idx_dialogueinfo_id_speaker_id` ON `dialogueinfo` (`id`, `speaker_id`);