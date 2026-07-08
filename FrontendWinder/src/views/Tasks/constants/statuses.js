/**
 * Статусы заданий
 */
export const STATUS_LABELS = {
    new: 'Новое',
    materials_requested: 'Материалы запрошены',
    materials_received: 'Материалы получены',
    submitted: 'Сдано',
    reported: 'Внесено в отчетность',
    archived: 'В архиве'
}

/**
 * Пошаговые переходы статусов (для мотальщика)
 */
export const STATUS_FLOW = {
    materials_requested: ['materials_received'],
    materials_received: ['submitted'],
    reported: ['archived'],
    new: [],
    submitted: [],
    archived: []
}

/**
 * Классы для бейджей статусов
 */
export const STATUS_CLASSES = {
    new: 'status-new',
    materials_requested: 'status-requested',
    materials_received: 'status-issued',
    submitted: 'status-completed',
    reported: 'status-reported',
    archived: 'status-archived'
}