document.addEventListener('DOMContentLoaded', async () => {
  await requireAuth();
  const tbody = document.getElementById('tbodyCourses');
  const form = document.getElementById('formCourse');
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
    const res = await apiFetch(`/api/v1/courses?q=${q}&page=${page}&pageSize=${pageSize}`);
    total = res.total;
    const items = res.items;
    tbody.innerHTML = '';
        for (const c of items) {
          const tr = document.createElement('tr');
          tr.innerHTML = `
            <td>${c.Id}</td>
            <td>${c.Nombre}</td>
            <td>${c.Descripcion || ''}</td>
            <td>
              <button data-edit="${c.Id}">Editar</button>
              <button data-del="${c.Id}">Borrar</button>
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
      const c = await apiFetch('/api/v1/courses/' + idEdit);
      document.getElementById('id').value = c.Id;
      document.getElementById('nombre').value = c.Nombre;
      document.getElementById('descripcion').value = c.Descripcion || '';
    }
    if (idDel) {
      if (!confirm('¿Borrar curso #' + idDel + '?')) return;
      await apiFetch('/api/v1/courses/' + idDel, { method: 'DELETE' });
      await load();
    }
  });

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const payload = {
      nombre: document.getElementById('nombres').value.trim(),
      descripcion: document.getElementById('descripcion').value.trim()
    };
    const id = document.getElementById('id').value;
    if (id) {
      await apiFetch('/api/v1/courses/' + id, { method: 'PUT', body: JSON.stringify(payload) });
    } else {
      await apiFetch('/api/v1/courses', { method: 'POST', body: JSON.stringify(payload) });
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
