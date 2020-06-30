<template>
  <!-- <img alt="Quasar logo" src="~assets/quasar-logo-full.svg" /> -->
  <div class="q-pa-md row justify-center">
    <div style="width: 100%; max-width: 400px">
      <q-input v-model="nombText" label="Nombre"></q-input>
      <q-input v-model="mensText" label="Mensaje"></q-input>
      <br />
      <q-btn
        color="primary"
        @click="EnviarMensaje()"
        label="Enviar"
        :disable="disable"
      ></q-btn>
      <q-btn
        label="Agregar a grupo"
        color="primary"
        @click="AddToGroup()"
      ></q-btn>
      <!-- <span>{{conexionLista ? "Conexión lista al HUB" : "Sin conexión al Hub"}}</span>
        <ul v-for="(chat, i) in listaMensajes" :key="i">
          <li>{{chat.nombre}} dijo {{chat.msj}}</li>
      </ul>-->
      <div
        style="width: 100%; max-width: 400px"
        v-for="(chat, i) in listaMensajes"
        :key="i"
      >
        <q-chat-message
          :name="chat.nombre"
          :text="[chat.msj]"
          :sent="chat.sent"
        ></q-chat-message>
      </div>
    </div>
  </div>
</template>

<script>
import { request } from "@isatol/fetchmodule";
const signalR = require("@aspnet/signalr");
let connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/chathub", {
    accessTokenFactory: () => localStorage.getItem("token")
  })
  .build();
export default {
  name: "PageIndex",
  data() {
    return {
      sent: false,
      conexionLista: false,
      nombText: "",
      mensText: "",
      disable: true,
      listaMensajes: []
    };
  },
  created() {
    this.Login();
    this.Inicializar();
  },
  computed: {
    lastElement() {
      let lastElement = this.listaMensajes[this.listaMensajes.length - 1];
      console.log(lastElement);
      return lastElement;
    }
  },
  mounted() {
    this.RecibirMensaje();
  },
  methods: {
    Inicializar() {
      connection
        .start()
        .then(some => {
          this.conexionLista = true;
          this.disable = false;
        })
        .catch(err => {
          console.error(err);
        });
    },
    RecibirMensaje() {
      let esIsa = this.nombText === "Isaías";
      connection.on("Message", (user, message, ConnectionId) => {
        console.log(ConnectionId);
        var msg = message
          .replace(/&/g, "&amp;")
          .replace(/</g, "&lt;")
          .replace(/>/g, "&gt;");
        let uuid;
        this.listaMensajes.push({
          nombre: user,
          msj: msg,
          sent: ConnectionId === sessionStorage.getItem("id")
        });
      });
    },
    EnviarMensaje() {
      connection
        .invoke("SendMessage", this.nombText, this.mensText)
        .catch(err =>
          console.error("Error al enviar mensaje: " + err.toString())
        );
    },
    Login() {
      request("http://localhost:5000/api/Auth/Login", {
        data: JSON.stringify({
          Nombre: "Isaías"
        }),
        headers: {
          "Content-Type": "application/json"
        },
        method: "post"
      }).then(response => {
        localStorage.setItem("token", response.token);
        console.log(response);
      });
    },
    AddToGroup() {
      let webSocketUrl = connection.connection.transport.webSocket.url;
      let splitUrl = webSocketUrl.split("?")[1].split("&");
      let idParam = splitUrl[0].substring(3);
      sessionStorage.setItem("id", idParam);
      request("http://localhost:5000/api/Auth/AddToGroup", {
        method: "get",
        params: {
          connID: idParam,
          grupo: "Ethel"
        },
        headers: {
          "Content-Type": "application/json"
        }
      }).then(console.log);
    },
    uuidv4() {
      return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function(
        c
      ) {
        var r = (Math.random() * 16) | 0,
          v = c == "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      });
    }
  }
};
</script>
