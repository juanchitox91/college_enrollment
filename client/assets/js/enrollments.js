document.addEventListener('DOMContentLoaded', async () => {
  await requireAuth();
  const tbody = document.getElementById('tbodyEnrollments');
  const form = document.getElementById('formEnrollment');
  const btnReset = document.getElementById('btnReset');

  const qInput = document.getElementById('q');
  const searchForm = document.getElementById('searchForm');
  const prevBtn = document.getElementById('prevPage');
  const nextBtn = document.getElementById('nextPage');
  const pageInfo = document.getElementById('pageInfo');
  let page = 1;
  const pageSize = 10;
  let total = 0;

  async function load() {
    const q = encodeURIComponent(qInput.value || '');
    const res = await apiFetch(`/api/v1/enrollments?q=${q}&page=${page}&pageSize=${pageSize}`);
    total = res.total;
    const items = res.items;
    tbody.innerHTML = '';
    for (const s of items) {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${s.Id}</td>
        <td>${s.StudentId}</td>
        <td>${s.CourseId}</td>
        <td>${s.FechaInscripcion || ''}</td>
        <td>
          <button data-edit="${s.Id}">Editar</button>
          <button data-del="${s.Id}">Borrar</button>
        </td>`;
      tbody.appendChild(tr);
    }
    const totalPages = Math.max(1, Math.ceil(total / pageSize));
    pageInfo.textContent = `Página ${page} de ${totalPages} — ${total} registros`;
    prevBtn.disabled = page <= 1;
    nextBtn.disabled = page >= totalPages;
  }

  tbody.addEventListener('click', async (e) => {
    const idEdit = e.target.getAttribute('data-edit');
    const idDel = e.target.getAttribute('data-del');
    if (idEdit) {
      const s = await apiFetch('/api/v1/enrollments/' + idEdit);
      document.getElementById('id').value = s.Id;
      document.getElementById('studentId').value = s.StudentId;
      document.getElementById('courseId').value = s.CourseId;
      document.getElementById('fecha_inscripcion').value = s.FechaInscripcion ? s.FechaInscripcion.substring(0,10) : ''
    }
    if (idDel) {
      if (!confirm('¿Borrar inscripcion? #' + idDel + '?')) return;
      await apiFetch('/api/v1/enrollments/' + idDel, { method: 'DELETE' });
      await load();
    }
  });

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const payload = {
      studentId: document.getElementById('studentId').value.trim(),
      courseId: document.getElementById('courseId').value.trim(),
      fechaInscripcion: document.getElementById('fecha_inscripcion').value.trim(),
    };
    const id = document.getElementById('id').value;
    if (id) {
      await apiFetch('/api/v1/enrollments/' + id, { method: 'PUT', body: JSON.stringify(payload) });
    } else {
      await apiFetch('/api/v1/enrollment', { method: 'POST', body: JSON.stringify(payload) });
    }
    form.reset();
    await load();
  });

  btnReset.addEventListener('click', () => form.reset());
  searchForm.addEventListener('submit', (e) => { e.preventDefault(); page = 1; load(); });
  prevBtn.addEventListener('click', () => { if (page > 1) { page--; load(); } });
  nextBtn.addEventListener('click', () => { page++; load(); });

  await load();
});
