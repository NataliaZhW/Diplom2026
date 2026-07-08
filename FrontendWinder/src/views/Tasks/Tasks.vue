<template>
    <div class="tasks-container">
        <h1>📋 Задания</h1>

        <TasksToolbar
            :is-master="isMaster"
            :selected-ids="selectedIds"
            :can-calculate="canCalculate"
            :can-report="canReport"
            @calculate="calculateMaterials"
            @submit-report="submitReport"
        />

        <TasksFilter
            :filter-status="filterStatus"
            :filter-type="filterType"
            :filter-winder="filterWinder"
            :is-master="isMaster"
            :winders="winders"
            :current-user-name="currentUserName"
            @update:filterStatus="filterStatus = $event"
            @update:filterType="filterType = $event"
            @update:filterWinder="filterWinder = $event"
            @refresh="loadTasks"
        />

        <TasksTable
            :tasks="filteredTasks"
            :loading="loading"
            :error="error"
            :is-master="isMaster"
            :selected-ids="selectedIds"
            :can-change-status="canChangeStatus"
            :get-next-status-label="getNextStatusLabel"
            :get-status-class="getStatusClass"
            :format-date="formatDate"
            @accept="acceptTask"
            @contextmenu="showContextMenu"
            @show-status-confirm="showStatusConfirm"
            @open-detail="openDetailModal"
            @update:selected-ids="selectedIds = $event" 
        />

        <!-- Модалка подтверждения -->
        <ConfirmModal
            :visible="confirmModalVisible"
            :task="confirmTask"
            :action="confirmAction"
            :next-status-label="confirmTask ? getNextStatusLabel(confirmTask) : ''"
            @confirm="confirmStatusChange"
            @cancel="cancelConfirm"
        />

        <!-- Контекстное меню -->
        <ContextMenu
            :visible="contextMenuVisible"
            :x="contextMenuX"
            :y="contextMenuY"
            @cancel-accept="cancelAccept"
            @close="closeContextMenu"
        />

        <TaskDetailModal
            :visible="detailModalVisible"
            :task="detailTask"
            :get-status-class="getStatusClass"
            :format-date="formatDate"
            @close="closeDetailModal"
        />
    </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useTasks } from './composables/useTasks'
import { useTaskActions } from './composables/useTaskActions'

import TasksToolbar from './components/TasksToolbar.vue'
import TasksFilter from './components/TasksFilter.vue'
import TasksTable from './components/TasksTable.vue'
import ConfirmModal from './components/ConfirmModal.vue'
import ContextMenu from './components/ContextMenu.vue'
import TaskDetailModal from './components/TaskDetailModal.vue'

// ============================================================
// КОМПОЗАБЛЫ
// ============================================================

const {
    tasks,
    winders,
    loading,
    error,
    filterStatus,
    filterType,
    filterWinder,
    selectedIds: tasksSelectedIds,
    currentUserId,
    isMaster,
    currentUserName,
    filteredTasks,
    getStatusClass,
    formatDate,
    loadTasks,
    init
} = useTasks()

const {
    selectedIds,
    contextMenuVisible,
    contextMenuX,
    contextMenuY,
    contextMenuTaskId,
    confirmModalVisible,
    confirmTask,
    confirmAction,
    canChangeStatus,
    getNextStatusLabel,
    canCalculate,
    canReport,
    showStatusConfirm,
    confirmStatusChange,
    cancelConfirm,
    acceptTask,
    cancelAccept,
    calculateMaterials,
    submitReport,
    showContextMenu,
    closeContextMenu,
    detailModalVisible,
    detailTask,
    openDetailModal,
    closeDetailModal
} = useTaskActions(tasks, currentUserId, isMaster, loadTasks)

// ============================================================
// ИНИЦИАЛИЗАЦИЯ
// ============================================================

onMounted(() => {
    init()
})
</script>

<style scoped>
.tasks-container {
    padding: 2rem;
    max-width: 1400px;
    margin: 0 auto;
}

h1 {
    color: #2c3e50;
    margin-bottom: 1.5rem;
}
</style>