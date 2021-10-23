FROM python:3

COPY requirements.txt .
RUN pip3 install -r requirements.txt

COPY . .

ENTRYPOINT [ "gunicorn", "-c", "gunicorn.conf.py", "checker:app" ]
