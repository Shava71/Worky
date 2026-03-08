import http from 'k6/http';
import { check, sleep } from 'k6';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.1/index.js';

const BASE_URL = 'http://localhost:5006';

export const options = {
  stages: [
    { duration: '1m', target: 200 }, 
    { duration: '2m', target: 500 }, 
    { duration: '3m', target: 1000 },  
    { duration: '2m', target: 500 },   
    { duration: '1m', target: 0 },   
  ],
  thresholds: {
    http_req_duration: ['p(95)<1000'],  
    http_req_failed: ['rate<0.05'],// допускается 5% ошибок при высокой нагрузке
  },
};

const queries = ['методика', 'тренер', 'разработчик', 'менеджер', 'программист'];

export default function() {
  const rand = Math.random();
  let url;
  
  if (rand < 0.6) {
    const query = queries[Math.floor(Math.random() * queries.length)];
    url = `${BASE_URL}/api/vacancies?AISearch=${encodeURIComponent(query)}&Page=1&PageSize=20`;
  } else if (rand < 0.8) {
    url = `${BASE_URL}/api/vacancies?AISearch=тренер&min_wantedSalary=40000&max_wantedSalary=80000&Page=1&PageSize=20`;
  } else {
    url = `${BASE_URL}/api/vacancies?AISearch=разработчик&education=4&Page=1&PageSize=20`;
  }
  
  const response = http.get(url);
  
  check(response, {
    'status is 200': (r) => r.status === 200,
  });
  
  sleep(0.5); 
}

export function handleSummary(data) {
  return {
    'k6-load-test-1000.html': htmlReport(data),
    stdout: textSummary(data, { indent: ' ', enableColors: true }),
  };
}